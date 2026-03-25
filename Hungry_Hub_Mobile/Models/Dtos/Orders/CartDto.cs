// Core/DTOs/Orders/CartDto.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Orders;


public class FlexibleNumberToStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            // Ако е число, конвертираме до стринг
            if (reader.TryGetInt32(out int intValue))
                return intValue.ToString();
            if (reader.TryGetDecimal(out decimal decValue))
                return decValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (reader.TryGetDouble(out double dblValue))
                return dblValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            // Ако е стринг, връщаме директно
            return reader.GetString() ?? "0";
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return "0";
        }

        return reader.GetString() ?? "0";
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

public class CartItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("article_id")]
    public int ArticleId { get; set; }

    private string? _img;

    [JsonPropertyName("article_img")]
    //public string? Img
    //{
    //    get => string.IsNullOrEmpty(_img) ? null : $"http://10.0.2.2:8000/media/{_img}";  // for local use
    //    set => _img = value;
    //}
    public string? Img
    {
        get => string.IsNullOrEmpty(_img) ? null : _img;
        set => _img = value;
    }

    [JsonPropertyName("article_name")]
    public string ArticleName { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    [JsonConverter(typeof(FlexibleNumberToStringConverter))]
    public string Price { get; set; } = "0.00";

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("total_price")]
    [JsonConverter(typeof(FlexibleNumberToStringConverter))]
    public string TotalPrice { get; set; } = "0.00";

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
    public decimal TotalPriceDecimal
    {
        get
        {
            if (decimal.TryParse(TotalPrice, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            return 0;
        }
    }
}
// За CartItem - съответства на CartItemSerializer
public class CartDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("items")]
    public List<CartItemDto> Items { get; set; } = new();

    [JsonPropertyName("subtotal")]
    [JsonConverter(typeof(FlexibleNumberToStringConverter))]
    public string Subtotal { get; set; } = "0.00";

    [JsonPropertyName("delivery_fee")]
    [JsonConverter(typeof(FlexibleNumberToStringConverter))]
    public string DeliveryFee { get; set; } = "0.00";

    [JsonPropertyName("total")]
    [JsonConverter(typeof(FlexibleNumberToStringConverter))]
    public string Total { get; set; } = "0.00";

    [JsonIgnore]
    public bool IsEmpty => Items == null || Items.Count == 0;

    [JsonIgnore]
    public decimal SubtotalDecimal
    {
        get
        {
            if (decimal.TryParse(Subtotal, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            return 0;
        }
    }

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
    public decimal TotalDecimal
    {
        get
        {
            if (decimal.TryParse(Total, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            return 0;
        }
    }
}

public class CartResponseDto
{
    [JsonPropertyName("user_profile_id")]
    public int UserProfileId { get; set; }

    [JsonPropertyName("cart")]
    public CartDto Cart { get; set; } = new();
}

// За добавяне на артикул в кошницата
public class AddToCartResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("cart")]
    public CartDto Cart { get; set; } = new();
}

// За изпращане към API-тоz
public class AddToCartRequestDto
{
    // Празен - article_id е в URL-то
}
