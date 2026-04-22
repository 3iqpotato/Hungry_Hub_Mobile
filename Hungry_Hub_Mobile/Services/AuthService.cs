using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Auth;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class AuthService : BaseApiService, IAuthService
{
    public AuthService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("========== LOGIN REQUEST ==========");
            System.Diagnostics.Debug.WriteLine($"URL: {ApiRoutes.Accounts.Login}");
            System.Diagnostics.Debug.WriteLine($"Email: {request.Email}");

            var response = await PostAsync<LoginRequestDto, LoginResponseDto>(
                ApiRoutes.Accounts.Login,
                request);

            System.Diagnostics.Debug.WriteLine("========== LOGIN RESPONSE ==========");
            System.Diagnostics.Debug.WriteLine($"Response null? {response == null}");

            if (response != null && !string.IsNullOrEmpty(response.Access))
                await TokenStorage.SaveLoginResponseAsync(response);

            return response;
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"========== HTTP ERROR ==========");
            System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}, Status: {ex.StatusCode}");
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"========== GENERAL ERROR ==========");
            System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
            throw;
        }
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("========== REGISTER REQUEST ==========");
            System.Diagnostics.Debug.WriteLine($"Email: {request.Email}, Type: {request.Type}");

            var response = await PostAsync<RegisterRequestDto, RegisterResponseDto>(
                ApiRoutes.Accounts.Register,
                request);

            System.Diagnostics.Debug.WriteLine($"Response null? {response == null}");

            if (response != null && !string.IsNullOrEmpty(response.Access))
            {
                await TokenStorage.SaveLoginResponseAsync(new LoginResponseDto
                {
                    Access = response.Access,
                    Refresh = response.Refresh,
                    User = response.User,
                    Next = response.Next,
                    ProfileId = response.ProfileId
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"========== REGISTER ERROR ==========");
            System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await TokenStorage.GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await PostAsync<RefreshTokenRequestDto, LoginResponseDto>(
                ApiRoutes.Accounts.RefreshToken,
                new RefreshTokenRequestDto { Refresh = refreshToken });

            if (response != null && !string.IsNullOrEmpty(response.Access))
            {
                await TokenStorage.SaveTokensAsync(response.Access, refreshToken);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при refresh: {ex.Message}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await PostAsync(ApiRoutes.Accounts.Logout);
        }
        finally
        {
            TokenStorage.RemoveTokens();
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await TokenStorage.GetAccessTokenAsync();
        System.Diagnostics.Debug.WriteLine($"Refresh endpoint: {ApiRoutes.Accounts.RefreshToken}");
        if (string.IsNullOrEmpty(token))
        {
            System.Diagnostics.Debug.WriteLine("❌ Няма token");
            return false;
        }

        if (TokenStorage.IsTokenValid(token))
        {
            System.Diagnostics.Debug.WriteLine("✅ Token е валиден");
            return true;
        }

        System.Diagnostics.Debug.WriteLine("⏰ Token изтекъл, опит за refresh...");
        var refreshed = await RefreshTokenAsync();

        if (!refreshed)
        {
            System.Diagnostics.Debug.WriteLine("🗑️ Refresh неуспешен, чистим tokens");
            TokenStorage.RemoveTokens();
            return false;
        }

        System.Diagnostics.Debug.WriteLine("✅ Refresh успешен");
        return true;
    }

    public async Task<UserAccountDto> GetCurrentUserAsync()
    {
        return await TokenStorage.GetUserAsync<UserAccountDto>();
    }
}