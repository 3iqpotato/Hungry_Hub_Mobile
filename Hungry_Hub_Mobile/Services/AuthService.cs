using System.Text.Json;
using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Auth;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class AuthService : BaseApiService, IAuthService
{
    // Конструктор - извиква базовия
    public AuthService() : base()
    {
    }

    // Вход
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("========== LOGIN REQUEST ==========");
            System.Diagnostics.Debug.WriteLine($"URL: {ApiRoutes.Accounts.Login}");
            System.Diagnostics.Debug.WriteLine($"Email: {request.Email}");
            System.Diagnostics.Debug.WriteLine($"Password: {request.Password}");

            var response = await PostAsync<LoginRequestDto, LoginResponseDto>(
                ApiRoutes.Accounts.Login,
                request);

            System.Diagnostics.Debug.WriteLine("========== LOGIN RESPONSE ==========");
            System.Diagnostics.Debug.WriteLine($"Response null? {response == null}");

            // Ако получим успешен response, запазваме tokens-те
            if (response != null && !string.IsNullOrEmpty(response.Access))
            {
                // 🔥 Първо запази целия response (това вече включва profile_id)
                await TokenStorage.SaveLoginResponseAsync(response);

                //// 🔥 После запази и user данните
                //if (response.User != null) Глупости от чата вече се прави всичко в токен сториджа няма нужда от тези неща само се повтаряше код!
                //{
                //    await TokenStorage.SaveUserAsync(response.User);
                //}

                //// 🔥 И отделно profile_id ако го има
                //if (response.ProfileId.HasValue)
                //{
                //    await TokenStorage.SaveProfileIdAsync(response.ProfileId.Value);
                //    System.Diagnostics.Debug.WriteLine($"✅ Запазен profile_id: {response.ProfileId.Value}");
                //}

                //if (!string.IsNullOrEmpty(response.Next))
                //{
                //    await TokenStorage.SaveNextRouteAsync(response.Next);
                //    System.Diagnostics.Debug.WriteLine($"✅ Запазен next route: {response.Next}");
                //}
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine("========== HTTP ERROR ==========");
            System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Status Code: {ex.StatusCode}");
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("========== GENERAL ERROR ==========");
            System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }

    // Регистрация
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("========== REGISTER REQUEST ==========");
            System.Diagnostics.Debug.WriteLine($"URL: {ApiRoutes.Accounts.Register}");
            System.Diagnostics.Debug.WriteLine($"Email: {request.Email}");
            System.Diagnostics.Debug.WriteLine($"Type: {request.Type}");

            var response = await PostAsync<RegisterRequestDto, RegisterResponseDto>(
                ApiRoutes.Accounts.Register,
                request);

            System.Diagnostics.Debug.WriteLine("========== REGISTER RESPONSE ==========");
            System.Diagnostics.Debug.WriteLine($"Response null? {response == null}");

            if (response != null && !string.IsNullOrEmpty(response.Access))
            {
                System.Diagnostics.Debug.WriteLine($"Access token: {response.Access?.Substring(0, 20)}...");
                System.Diagnostics.Debug.WriteLine($"Next: {response.Next}");
                System.Diagnostics.Debug.WriteLine($"Profile ID: {response.ProfileId}");

                // Запазваме всичко в TokenStorage
                await TokenStorage.SaveLoginResponseAsync(new LoginResponseDto
                {
                    Access = response.Access,
                    Refresh = response.Refresh,
                    User = response.User,
                    Next = response.Next,
                    ProfileId = response.ProfileId
                });
            }

            //if (response.ProfileId.HasValue)
            //{
            //    await TokenStorage.SaveProfileIdAsync(response.ProfileId.Value);
            //    System.Diagnostics.Debug.WriteLine($"✅ Запазен profile_id от register: {response.ProfileId.Value}");
            //}

            //if (response.User != null)
            //{
            //    await TokenStorage.SaveUserAsync(response.User);
            //}  повтарящ се код не ни трява 

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"========== REGISTER ERROR ==========");
            System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
            throw;
        }
    }

    // Опресняване на token
    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var oldUserJson = await SecureStorage.Default.GetAsync("user_data");
            System.Diagnostics.Debug.WriteLine($"[BEFORE REFRESH] user_data = {oldUserJson}");
            var refreshToken = await TokenStorage.GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var request = new RefreshTokenRequestDto
            {
                Refresh = refreshToken
            };

            // ПРАЩАМЕ: POST /api/accounts/token/refresh/
            // с body: { "refresh": "..." }
            var response = await PostAsync<RefreshTokenRequestDto, LoginResponseDto>(
                ApiRoutes.Accounts.RefreshToken,
                request);

            if (response != null && !string.IsNullOrEmpty(response.Access))
            {
                // Запазваме новия access token (refresh token остава същия)
                await TokenStorage.SaveTokensAsync(response.Access, refreshToken);

                //if (response.User != null)
                //{
                //    System.Diagnostics.Debug.WriteLine($"не би трябвало да влизаме тук!!!");
                //    await TokenStorage.SaveUserAsync(response.User);
                //}
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

    // Изход
    public async Task LogoutAsync()
    {
        try
        {
            // ПРАЩАМЕ: POST /api/accounts/logout/
            await PostAsync(ApiRoutes.Accounts.Logout);
        }
        finally
        {
            // Дори logout заявката да не успее, чистим локалните tokens
            TokenStorage.RemoveTokens();
        }
    }

    // Проверка дали е логнат и рефрешване ако токена е изтекъл тук е сложена защото е по добре тук отколкото в сториджа 
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await TokenStorage.GetAccessTokenAsync();

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

        System.Diagnostics.Debug.WriteLine("⏰ Token-ът е изтекъл, опит за refresh...");
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

    // Взема текущия потребител
    public async Task<UserAccountDto> GetCurrentUserAsync()
    {
        return await TokenStorage.GetUserAsync<UserAccountDto>();
    }
}