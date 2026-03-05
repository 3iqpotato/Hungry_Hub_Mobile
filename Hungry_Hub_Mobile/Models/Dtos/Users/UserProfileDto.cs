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

    [JsonPropertyName("img")]
    public string? Img { get; set; }

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

    [JsonPropertyName("img")]
    public string? Img { get; set; }
}

// Това е за home page на user - събира информация от различни източници
//public class UserHomeDto
//{
//    [JsonPropertyName("profile")]
//    public UserProfileDto Profile { get; set; }


//    [JsonPropertyName("recent_orders")]
//    public List<OrderDto>? RecentOrders { get; set; }  // Слагаме ? защото може да няма скорошни поръчки

//    [JsonPropertyName("recommended_restaurants")]
//    public List<RestaurantMiniDto> RecommendedRestaurants { get; set; }
//}