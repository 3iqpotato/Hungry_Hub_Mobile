using Hungry_Hub_Mobile.Core.DTOs.Users;
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface IUserProfileService
{
    /// <summary>
    /// Взема текущия профил на потребителя
    /// </summary>
    Task<UserProfileDto> GetProfileAsync();

    /// <summary>
    /// РЕДАКТИРА съществуващ профил (PUT/PATCH)
    /// </summary>
    Task<UserProfileDto> EditProfileAsync(UpdateUserProfileDto profile,
        byte[]? imageBytes = null,      // ← добави
        string? imageFileName = null); 
    /// <summary>
    /// Създава или обновява профила
    /// </summary>
    Task<CompleteProfileResponseDto> UpdateProfileAsync(UpdateUserProfileDto profile,
    byte[]? imageBytes = null,
    string? imageFileName = null);

    Task<int?> GetCurrentProfileIdAsync();
}

