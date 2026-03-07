// Core/DTOs/Articles/ArticleDto.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Articles;

public class FlexibleNumberConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            // Ако е number, го конвертираме до string
            if (reader.TryGetInt32(out int intValue))
                return intValue.ToString();
            if (reader.TryGetDouble(out double doubleValue))
                return doubleValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            // Ако е string, го връщаме директно
            return reader.GetString() ?? string.Empty;
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return reader.GetString() ?? string.Empty;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

// Това съответства на ArticleSerializer
public class ArticleDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    private string? _img;

    [JsonPropertyName("img")]
    public string? Img
    {
        get => string.IsNullOrEmpty(_img) ? null : $"http://10.0.2.2:8000{_img}";
        set => _img = value;
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("ingredients")]
    public string? Ingredients { get; set; }

    // 🔥 ПРОМЕНИ ОТ decimal НА string
    [JsonPropertyName("price")]
    public string Price { get; set; } = "0.00";

    [JsonPropertyName("rating")]
    public double? Rating { get; set; }

    // 🔥 ПРОМЕНИ ОТ double? НА string?
    [JsonPropertyName("weight")]
    [JsonConverter(typeof(FlexibleNumberConverter))]
    public string? Weight { get; set; }

    [JsonPropertyName("menu")]
    public int MenuId { get; set; }

    [JsonPropertyName("restaurant_id")]
    public int RestaurantId { get; set; }

    [JsonPropertyName("restaurant_name")]
    public string RestaurantName { get; set; } = string.Empty;

    // Helper properties за UI
    [JsonIgnore]
    public decimal PriceDecimal
    {
        get
        {
            if (decimal.TryParse(Price, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            return 0;
        }
    }

    [JsonIgnore]
    public double? WeightDouble
    {
        get
        {
            if (!string.IsNullOrEmpty(Weight) &&
                double.TryParse(Weight, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }
    }
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