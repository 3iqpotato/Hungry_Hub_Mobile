using System.Text.Json.Serialization;
using Hungry_Hub_Mobile.Core.DTOs.Articles;

namespace Hungry_Hub_Mobile.Core.DTOs.Restaurants;

public class MenuForUsersDto
{
    [JsonPropertyName("restaurant")]
    public RestaurantMiniDto Restaurant { get; set; } = new();

    [JsonPropertyName("menu")]
    public MenuDto Menu { get; set; } = new();

    [JsonPropertyName("articles")]
    public List<ArticleDto> Articles { get; set; } = new();

    [JsonPropertyName("selected_type")]
    public string? SelectedType { get; set; }
}

// DTO за заявка с филтър
public class MenuFilterDto
{
    public int RestaurantId { get; set; }
    public string? FoodType { get; set; }
}