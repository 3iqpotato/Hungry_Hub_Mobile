using System.Text;
using System.Text.Json;
using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Core.Helpers;

namespace Hungry_Hub_Mobile.Services;

public abstract class BaseApiService
{
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonOptions;

    protected BaseApiService()
    {
        // Създаваме HttpClient с нашия handler, който добавя token
        var handler = new AuthenticatedHttpClientHandler();
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(AppConstants.FullBaseApiUrl)
        };

        // Настройки за JSON - важно за да се мапнат правилно пропъртитата
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, // Не прави разлика между main и MAIN
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // очакваме JSON с camelCase
        };
    }

    // Helper method за GET заявки
    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        await EnsureSuccessStatusCode(response);

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    // Helper method за POST заявки с body
    //protected async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    //{
    //    var json = JsonSerializer.Serialize(data, _jsonOptions);
    //    var content = new StringContent(json, Encoding.UTF8, "application/json");

    //    var response = await _httpClient.PostAsync(endpoint, content);
    //    await EnsureSuccessStatusCode(response);

    //    var responseJson = await response.Content.ReadAsStringAsync();
    //    return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
    //}
    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);

            // 🔍 Какво изпращаме
            System.Diagnostics.Debug.WriteLine($"--- POST Request ---");
            System.Diagnostics.Debug.WriteLine($"Full URL: {_httpClient.BaseAddress}{endpoint}");
            System.Diagnostics.Debug.WriteLine($"Request JSON: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 🔍 Проверка за token
            var token = await TokenStorage.GetAccessTokenAsync();
            System.Diagnostics.Debug.WriteLine($"Token present: {!string.IsNullOrEmpty(token)}");

            var response = await _httpClient.PostAsync(endpoint, content);

            // 🔍 Какво получихме
            System.Diagnostics.Debug.WriteLine($"Response Status: {(int)response.StatusCode} {response.StatusCode}");

            var responseJson = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Response JSON: {responseJson}");

            await EnsureSuccessStatusCode(response);

            var result = JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);

            if (result is CartDto cart)
            {
                System.Diagnostics.Debug.WriteLine($"=== ДЕСЕРИАЛИЗИРАН CART ===");
                System.Diagnostics.Debug.WriteLine($"Cart ID: {cart.Id}");
                System.Diagnostics.Debug.WriteLine($"Items count: {cart.Items?.Count ?? 0}");

                if (cart.Items != null)
                {
                    foreach (var item in cart.Items)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - Item ID: {item.Id}, Name: {item.ArticleName}");
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"POST Error: {ex.Message}");
            throw;
        }
    }

    // Helper method за POST без очакван response (например logout)
    protected async Task PostAsync(string endpoint)
    {
        var response = await _httpClient.PostAsync(endpoint, null);
        await EnsureSuccessStatusCode(response);
    }

    // Helper method за PUT заявки
    protected async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(endpoint, content);
        await EnsureSuccessStatusCode(response);

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
    }

    // Helper method за DELETE
    protected async Task DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        await EnsureSuccessStatusCode(response);
    }

    // Проверка за успешен статус
    private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {error}");
        }
    }
}