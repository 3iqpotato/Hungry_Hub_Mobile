// Core/DTOs/Restaurants/RestaurantDto.cs
using Hungry_Hub_Mobile.Core.DTOs.Articles;
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Restaurants;


public class RestaurantProfileResponseDto
{
    [JsonPropertyName("profile_exists")]
    public bool ProfileExists { get; set; }

    [JsonPropertyName("restaurant")]
    public RestaurantDetailDto? Restaurant { get; set; }
}

public class UpdateRestaurantProfileDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("img")]
    public string? Img { get; set; }

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
}

public class CompleteRestaurantResponseDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("restaurant")]
    public RestaurantDetailDto Restaurant { get; set; } = new();

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("restaurant_id")]
    public int RestaurantId { get; set; }
}


// Mini версия за списъци - съответства на RestaurantMiniSerializer
public class RestaurantMiniDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("img")]
    public string Img { get; set; }

    //[JsonPropertyName("rating")]
    //public double Rating { get; set; }

    //[JsonPropertyName("delivery_fee")]
    //public decimal DeliveryFee { get; set; }

    //[JsonPropertyName("discount")]
    //public decimal? Discount { get; set; }
}

// Пълен детайл за ресторант - съответства на RestaurantSerializer
public class RestaurantDetailDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("img")]
    public string Img { get; set; }

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("delivery_fee")]
    public string DeliveryFee { get; set; } = "0.00";

    [JsonPropertyName("discount")]
    public string? Discount { get; set; }

    // Това няма го в serializer-а, но можем да го добавим ако ни трябва
    [JsonPropertyName("menu")]
    public MenuDto? Menu { get; set; }

    // Helper property за употреба в UI - конвертира string към decimal
    [JsonIgnore]
    public decimal DeliveryFeeDecimal
    {
        get
        {
            if (decimal.TryParse(DeliveryFee, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            return 0;
        }
    }

    [JsonIgnore]
    public decimal? DiscountDecimal
    {
        get
        {
            if (!string.IsNullOrEmpty(Discount) &&
                decimal.TryParse(Discount, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }
    }
}

// Menu DTO - съответства на MenuSerializer
public class MenuDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("restaurant")]
    public int RestaurantId { get; set; }  // В Django е restaurant (ID)

    [JsonPropertyName("restaurant_id")]
    public int? RestaurantIdAlt { get; set; } // От source="restaurant.id" TODO moze da e izlishno

    [JsonPropertyName("restaurant_name")]
    public string? RestaurantName { get; set; }

    // За удобство - articles-те на това меню (няма го в serializer-а)
    public List<ArticleDto>? Articles { get; set; }
}

// За edit на restaurant
public class RestaurantUpdateDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("img")]
    public string Img { get; set; }

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("delivery_fee")]
    public decimal DeliveryFee { get; set; }

    [JsonPropertyName("discount")]
    public decimal? Discount { get; set; }
}

// За home page на ресторант
public class RestaurantHomeDto
{
    [JsonPropertyName("restaurant")]
    public RestaurantDetailDto Restaurant { get; set; }

    [JsonPropertyName("today_orders")]
    public int TodayOrdersCount { get; set; }

    [JsonPropertyName("pending_orders")]
    public int PendingOrdersCount { get; set; }
}