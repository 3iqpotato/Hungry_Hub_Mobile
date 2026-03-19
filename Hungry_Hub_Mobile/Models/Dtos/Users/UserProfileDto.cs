// Core/DTOs/Users/UserProfileDto.cs
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Users;

// За профила на обикновен потребител
public class UserProfileDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    private string? _img;

    [JsonPropertyName("img")]
    public string? Img
    {
        get => string.IsNullOrEmpty(_img) ? null : $"http://10.0.2.2:8000{_img}";
        set => _img = value;
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; } = string.Empty;

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string? Address { get; set; } = string.Empty;

    [JsonPropertyName("profile_completed")]
    public bool ProfileCompleted { get; set; }
}

// За edit/update на профил
public class UpdateUserProfileDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
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

public class EditProfileResponseDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("profile")]
    public UserProfileDto Profile { get; set; } = new();
}