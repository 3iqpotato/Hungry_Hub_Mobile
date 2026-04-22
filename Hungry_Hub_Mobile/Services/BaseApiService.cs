using System.Text;
using System.Text.Json;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Core.Helpers;

namespace Hungry_Hub_Mobile.Services;

public abstract class BaseApiService
{
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonOptions;

    // HttpClient идва от DI - вече не го създаваме сами
    protected BaseApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        await EnsureSuccessStatusCode(response);

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);

            System.Diagnostics.Debug.WriteLine($"--- POST Request ---");
            System.Diagnostics.Debug.WriteLine($"Full URL: {_httpClient.BaseAddress}{endpoint}");
            System.Diagnostics.Debug.WriteLine($"Request JSON: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await TokenStorage.GetAccessTokenAsync();
            System.Diagnostics.Debug.WriteLine($"Token present: {!string.IsNullOrEmpty(token)}");

            var response = await _httpClient.PostAsync(endpoint, content);

            System.Diagnostics.Debug.WriteLine($"Response Status: {(int)response.StatusCode} {response.StatusCode}");

            var responseJson = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Response JSON: {responseJson}");

            await EnsureSuccessStatusCode(response);

            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"POST Error: {ex.Message}");
            throw;
        }
    }

    protected async Task PostAsync(string endpoint)
    {
        var response = await _httpClient.PostAsync(endpoint, null);
        await EnsureSuccessStatusCode(response);
    }

    protected async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(endpoint, content);
        await EnsureSuccessStatusCode(response);

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
    }

    protected async Task DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        await EnsureSuccessStatusCode(response);
    }

    private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {error}");
        }
    }

    protected string ParseDjangoError(string errorJson)
    {
        try
        {
            var doc = JsonDocument.Parse(errorJson);
            var messages = new List<string>();

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.Value.EnumerateArray())
                        messages.Add(item.GetString() ?? "");
                }
                else if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    messages.Add(prop.Value.GetString() ?? "");
                }
            }

            return messages.Count > 0
                ? string.Join("\n", messages)
                : "Възникна грешка.";
        }
        catch
        {
            return "Възникна грешка.";
        }
    }
}