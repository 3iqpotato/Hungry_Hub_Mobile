using System.Text.Json;
using Hungry_Hub_Mobile.Core.DTOs.Auth;
using Microsoft.Maui.Storage;
using System.IdentityModel.Tokens.Jwt;
using Hungry_Hub_Mobile.Services.Interfaces;  // Инсталирай NuGet: System.IdentityModel.Tokens.Jwt
namespace Hungry_Hub_Mobile.Core.Helpers;

public static class TokenStorage
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string UserKey = "user_data";
    private const string UserTypeKey = "user_type";
    private const string HasProfileKey = "has_profile";
    private const string NextRouteKey = "next_route";
    private const string ProfileIdKey = "profile_id";
    public static async Task SaveLoginResponseAsync(LoginResponseDto response)
    {
        try
        {
            await SecureStorage.Default.SetAsync(AccessTokenKey, response.Access);
            await SecureStorage.Default.SetAsync(RefreshTokenKey, response.Refresh);

            if (response.User != null)
            {
                await SecureStorage.Default.SetAsync(UserKey, JsonSerializer.Serialize(response.User));
                await SecureStorage.Default.SetAsync(UserTypeKey, response.User.Type);
            }

            if (response.ProfileId.HasValue)
                await SecureStorage.Default.SetAsync(ProfileIdKey, response.ProfileId.Value.ToString());

            if (!string.IsNullOrEmpty(response.Next))
                await SecureStorage.Default.SetAsync(NextRouteKey, response.Next);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Грешка при запазване: {ex.Message}");
        }
    }


    public static async Task<T> GetUserAsync<T>()
    {
        try
        {
            var json = await SecureStorage.Default.GetAsync(UserKey);
            if (string.IsNullOrEmpty(json))
            {
                System.Diagnostics.Debug.WriteLine($"ℹ️ Няма запазени user данни");
                return default;
            }

            var result = JsonSerializer.Deserialize<T>(json);
            System.Diagnostics.Debug.WriteLine($"✅ User data loaded: {typeof(T).Name}");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при вземане на user: {ex.Message}");
            return default;
        }
    }
    public static async Task<string> GetUserTypeAsync()
    {
        return await SecureStorage.Default.GetAsync(UserTypeKey);
    }

    public static async Task<bool> HasCompleteProfileAsync()
    {
        var hasProfile = await SecureStorage.Default.GetAsync(HasProfileKey);
        return hasProfile == "True";
    }

    public static async Task<string> GetNextRouteAsync()
    {
        return await SecureStorage.Default.GetAsync(NextRouteKey);
    }

    /// <summary>
    /// Запазва tokens след login/register
    /// </summary>
    /// 

    public static async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        try
        {
            await SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);
            await SecureStorage.Default.SetAsync(RefreshTokenKey, refreshToken);
        }
        catch (Exception ex)
        {
            // Ако има проблем със SecureStorage (рядко), логваме
            System.Diagnostics.Debug.WriteLine($"Грешка при запазване на tokens: {ex.Message}");
        }
    }

    public static async Task<string> GetAccessTokenAsync()
    {
        try
        {
            return await SecureStorage.Default.GetAsync(AccessTokenKey);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<string> GetRefreshTokenAsync()
    {
        try
        {
            return await SecureStorage.Default.GetAsync(RefreshTokenKey);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Проверява дали потребителя е логнат (има access token)
    /// </summary>
    public static async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetAccessTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            System.Diagnostics.Debug.WriteLine("❌ Няма token");
            return false;
        }

        if (!IsTokenValid(token))
        {
            System.Diagnostics.Debug.WriteLine("⏰ Token-ът е изтекъл");
            return false;
        }

        System.Diagnostics.Debug.WriteLine("✅ Token е валиден");
        return true;
    }

    /// <summary>
    /// Изтрива всички tokens (при logout)
    /// </summary>
    public static void RemoveTokens()
    {
        try
        {
            SecureStorage.Default.Remove(AccessTokenKey);
            SecureStorage.Default.Remove(RefreshTokenKey);
            SecureStorage.Default.Remove(UserKey);
            SecureStorage.Default.Remove(UserTypeKey);
            SecureStorage.Default.Remove(HasProfileKey);
            SecureStorage.Default.Remove(NextRouteKey);
            SecureStorage.Default.Remove(ProfileIdKey);  // ← добави това
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Грешка при изтриване на tokens: {ex.Message}");
        }
    }

    /// <summary>
    /// Запазва данните на потребителя (опционално)
    /// </summary>
    public static async Task SaveUserAsync<T>(T user)
    {
        try
        {
            var json = JsonSerializer.Serialize(user);
            await SecureStorage.Default.SetAsync(UserKey, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Грешка при запазване на user: {ex.Message}");
        }
    }

    /// <summary>
    /// Взема запазените данни на потребителя
    /// </summary>


    public static async Task SaveProfileIdAsync(int profileId)
    {
        try
        {
            await SecureStorage.Default.SetAsync(ProfileIdKey, profileId.ToString());
            System.Diagnostics.Debug.WriteLine($"✅ Запазен profile_id в storage: {profileId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при запазване на profile_id: {ex.Message}");
        }
    }

    public static async Task<int?> GetProfileIdAsync()
    {
        try
        {
            var value = await SecureStorage.Default.GetAsync(ProfileIdKey);
            if (int.TryParse(value, out int result))
            {
                System.Diagnostics.Debug.WriteLine($"✅ Взет profile_id от storage: {result}");
                return result;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при вземане на profile_id: {ex.Message}");
        }

        return null;
    }

    public static async Task SaveNextRouteAsync(string nextRoute)
    {
        try
        {
            await SecureStorage.Default.SetAsync(NextRouteKey, nextRoute);
            System.Diagnostics.Debug.WriteLine($"✅ Запазен next route: {nextRoute}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при запазване на next route: {ex.Message}");
        }
    }


    // тук локално проверяваме дали токена е изтекъл за да не товаря сървърчето че в azure не е много евтино :)
public static bool IsTokenValid(string token)
{
    if (string.IsNullOrEmpty(token))
        return false;
    
    try
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        // Провери дали token-ът е изтекъл
        var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (exp != null)
        {
            var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
            if (expDate < DateTime.UtcNow)
            {
                System.Diagnostics.Debug.WriteLine("⏰ Token-ът е изтекъл");
                return false;
            }
        }
        
        return true;
    }
    catch
    {
        return false;
    }
}


}