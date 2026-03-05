// Core/DTOs/Articles/ArticleDto.cs
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Articles;

// Това съответства на ArticleSerializer
public class ArticleDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("img")]
    public string? Img { get; set; }  // URL към изображението

    [JsonPropertyName("type")]
    public string? Type { get; set; } // 'salads', 'appetizers', etc.

    [JsonPropertyName("ingredients")]
    public string? Ingredients { get; set; }

    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("rating")]
    public double? Rating { get; set; }

    [JsonPropertyName("weight")]
    public double? Weight { get; set; }

    [JsonPropertyName("menu")]
    public int MenuId { get; set; }  // Това идва от menu_id в serializer-а

    [JsonPropertyName("restaurant_id")]
    public int RestaurantId { get; set; }

    [JsonPropertyName("restaurant_name")]
    public string? RestaurantName { get; set; }
}

// За създаване/редактиране на article - съответства на ArticleCreateUpdateSerializer
public class ArticleCreateUpdateDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("img")]
    public string? Img { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("ingredients")]
    public string? Ingredients { get; set; }

    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("rating")]
    public double? Rating { get; set; }

    [JsonPropertyName("weight")]
    public double? Weight { get; set; }

    [JsonPropertyName("menu")]
    public int MenuId { get; set; }  // В Django е menu (ID), тук го именуваме MenuId
}

// За enum стойностите на type (може да ползваме за dropdown в UI)
public static class ArticleTypes
{
    public const string Salads = "salads";
    public const string Appetizers = "appetizers";
    public const string MainCourse = "main_course";
    public const string Desserts = "desserts";

    // За показване в UI - хубави имена
    public static string GetDisplayName(string type)
    {
        return type switch
        {
            Salads => "Салати",
            Appetizers => "Предястия",
            MainCourse => "Основни ястия",
            Desserts => "Десерти",
            _ => type
        };
    }
}