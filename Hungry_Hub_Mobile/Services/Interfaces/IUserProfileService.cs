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
    Task<UserProfileDto> EditProfileAsync(UpdateUserProfileDto profile);

    /// <summary>
    /// Създава или обновява профила
    /// </summary>
    Task<CompleteProfileResponseDto> UpdateProfileAsync(UpdateUserProfileDto profile);
}

public class CompleteProfileResponseDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public UserProfileDto? User { get; set; }

    [JsonPropertyName("profile")]
    public UserProfileDto? Profile { get; set; }

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("profileid")]
    public int? ProfileId { get; set; }
}