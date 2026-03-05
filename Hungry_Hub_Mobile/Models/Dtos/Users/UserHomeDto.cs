using System.Text.Json.Serialization;
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;

namespace Hungry_Hub_Mobile.Core.DTOs.Users;

public class UserHomeDto
{
    [JsonPropertyName("profile")]
    public UserProfileDto Profile { get; set; } = new();

    [JsonPropertyName("restaurants")]
    public List<RestaurantMiniDto> Restaurants { get; set; } = new();
}