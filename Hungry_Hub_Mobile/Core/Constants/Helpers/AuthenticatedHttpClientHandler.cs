using Hungry_Hub_Mobile.Services.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

namespace Hungry_Hub_Mobile.Core.Helpers;

public class AuthenticatedHttpClientHandler : HttpClientHandler
{
    private const int MaxRetries = 1; // Само един опит за refresh
    private int _retryCount = 0; /// added limit!
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Клонираме ПРЕДИ първото изпращане, за да запазим Content-а за евентуален retry
        var clonedForRetry = await CloneHttpRequestMessageAsync(request);

        var response = await SendWithTokenAsync(request, cancellationToken); // използваме оригинала

        var isAuthEndpoint = request.RequestUri?.ToString().Contains("/accounts/login/") == true ||
                             request.RequestUri?.ToString().Contains("/accounts/register/") == true ||
                             request.RequestUri?.ToString().Contains("/accounts/token/refresh/") == true;

        if (response.StatusCode == HttpStatusCode.Unauthorized && !isAuthEndpoint && _retryCount < MaxRetries)
        {
            Debug.WriteLine("🔑 Получен 401 - опит за refresh...");
            response.Dispose();
            _retryCount++;

            var refreshSuccess = await TryRefreshTokenAsync();
            if (refreshSuccess)
            {
                Debug.WriteLine("✅ Token обновен, retry...");
                response = await SendWithTokenAsync(clonedForRetry, cancellationToken); // използваме клонинга
            }
            else
            {
                clonedForRetry.Dispose(); // почистваме ако не се използва
                Debug.WriteLine("❌ Refresh failed");
                await HandleRefreshFailureAsync();
            }
        }
        else
        {
            clonedForRetry.Dispose(); // не ни трябва, почистваме
        }

        _retryCount = 0;
        return response;
    }

    private async Task<HttpResponseMessage> SendWithTokenAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await TokenStorage.GetAccessTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<bool> TryRefreshTokenAsync()
    {
        try
        {
            var refreshToken = await TokenStorage.GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
            {
                Debug.WriteLine("❌ Няма refresh token");
                return false;
            }

            Debug.WriteLine("🔄 Опит за refresh на token...");

            // 🔥 ВАЖНО: Използваме base.SendAsync за да заобиколим handler-a
            var request = new HttpRequestMessage(HttpMethod.Post, "api/accounts/token/refresh/");

            var body = System.Text.Json.JsonSerializer.Serialize(new { refresh = refreshToken });
            request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

            var response = await base.SendAsync(request, CancellationToken.None);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"❌ Refresh failed: {response.StatusCode} - {json}");
                return false;
            }

            using var doc = System.Text.Json.JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("access", out var accessToken))
            {
                var newAccessToken = accessToken.GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(newAccessToken)) return false;
                await TokenStorage.SaveTokensAsync(newAccessToken, refreshToken);
                Debug.WriteLine("✅ Token обновен успешно");
                return true;
            }

            Debug.WriteLine("❌ Няма 'access' property в response");
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Грешка при refresh: {ex.Message}");
            return false;
        }
    }

    private async Task HandleRefreshFailureAsync()
    {
        // Изчисти всички tokens
        TokenStorage.RemoveTokens();

        // Пренасочи към login страницата на главния UI thread
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var navigationService = MauiProgram.Services.GetService<INavigationService>();
            if (navigationService != null)
            {
                // Вземи текущата страница и избери как да пренасочиш
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    // Изчисти целия навигационен стек и отиди на login
                    await navPage.Navigation.PopToRootAsync();  
                }
                await navigationService.GoToAsync("login");
            }
        });
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (request.Content != null)
        {
            // Запази оригиналния Content-Type
            var contentType = request.Content.Headers.ContentType?.ToString();

            if (contentType != null && contentType.Contains("multipart/form-data"))
            {
                // Multipart не може да се клонира като стринг — прочети като байтове
                var bytes = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(bytes);
            }
            else
            {
                var content = await request.Content.ReadAsStringAsync();
                clone.Content = new StringContent(
                    content,
                    System.Text.Encoding.UTF8,
                    "application/json");
            }

            // Копирай ВСИЧКИ content headers от оригинала (включително boundary за multipart)
            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}