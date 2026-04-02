using System.Net.Http.Headers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Core.Helpers;

public class AuthenticatedHttpClientHandler : HttpClientHandler
{
    private const int MaxRetries = 1; // Само един опит за refresh

    protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
    {
        var clonedRequest = await CloneHttpRequestMessageAsync(request);
        var response = await SendWithTokenAsync(clonedRequest, cancellationToken);

        // Не опитвай refresh на login/register
        var isAuthEndpoint = request.RequestUri?.ToString().Contains("/accounts/login/") == true ||
                             request.RequestUri?.ToString().Contains("/accounts/register/") == true;

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !isAuthEndpoint)
        {
            System.Diagnostics.Debug.WriteLine("🔑 Получен 401 Unauthorized - опит за refresh...");
            response.Dispose();

            var refreshSuccess = await TryRefreshTokenAsync();
            if (refreshSuccess)
            {
                System.Diagnostics.Debug.WriteLine("✅ Token-ът е обновен, повторен опит на заявката...");
                var retryRequest = await CloneHttpRequestMessageAsync(request);
                response = await SendWithTokenAsync(retryRequest, cancellationToken);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Refresh failed - redirect to login");
                await HandleRefreshFailureAsync();
            }
        }

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
            var authService = MauiProgram.Services.GetService<IAuthService>();
            if (authService != null)
            {
                return await authService.RefreshTokenAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при refresh: {ex.Message}");
        }

        return false;
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
                    await navigationService.GoToAsync("login");
                }
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