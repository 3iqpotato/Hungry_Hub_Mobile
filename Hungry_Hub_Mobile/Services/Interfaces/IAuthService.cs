using Hungry_Hub_Mobile.Core.DTOs.Auth;

namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Вход на потребител
    /// </summary>
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Регистрация на нов потребител
    /// </summary>
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);  // Промени тук

    /// <summary>
    /// Опресняване на token
    /// </summary>
    Task<bool> RefreshTokenAsync();

    /// <summary>
    /// Изход
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    /// Проверка дали потребителя е логнат
    /// </summary>
    Task<bool> IsAuthenticatedAsync();

    /// <summary>
    /// Взема текущия потребител
    /// </summary>
    Task<UserAccountDto> GetCurrentUserAsync();
}