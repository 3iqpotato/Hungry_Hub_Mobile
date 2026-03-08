// Core/DTOs/Orders/OrderDto.cs
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Orders;

// Статуси на поръчка
public static class OrderStatus
{
    public const string Pending = "pending";
    public const string ReadyForPickup = "ready_for_pickup";
    public const string OnDelivery = "on_delivery";
    public const string Delivered = "delivered";

    public static string GetDisplayName(string status)
    {
        return status switch
        {
            Pending => "Чакаща",
            ReadyForPickup => "Готова за вземане",
            OnDelivery => "В доставка",
            Delivered => "Доставена",
            _ => status
        };
    }
}

public class CreateOrderResponseDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public OrderDto Order { get; set; } = new();

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("order_id")]
    public int OrderId { get; set; }
}


// За OrderItem - съответства на OrderItemSerializer
public class OrderItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("article_id")]
    public int ArticleId { get; set; }

    [JsonPropertyName("article_name")]
    public string ArticleName { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public string Price { get; set; } = "0.00";

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("total_price")]
    public decimal TotalPrice { get; set; }

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
}


// За поръчка - съответства на OrderSerializer
public class OrderDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("address_for_delivery")]
    public string AddressForDelivery { get; set; } = string.Empty;

    [JsonPropertyName("order_date_time")]
    public DateTime OrderDateTime { get; set; }

    [JsonPropertyName("delivery_time")]
    public DateTime? DeliveryTime { get; set; }

    [JsonPropertyName("delivery_fee")]
    public string DeliveryFee { get; set; } = "0.00";

    [JsonPropertyName("total_price")]
    public string TotalPrice { get; set; } = "0.00";

    [JsonPropertyName("restaurant")]
    public int? RestaurantId { get; set; }

    [JsonPropertyName("restaurant_name")]
    public string? RestaurantName { get; set; }

    [JsonPropertyName("supplier")]
    public int? SupplierId { get; set; }

    [JsonPropertyName("supplier_name")]
    public string? SupplierName { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItemDto> Items { get; set; } = new();

    // Helper properties
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

    public string StatusDisplayName => GetStatusDisplayName(Status);
    public string FormattedTotal => $"{TotalPriceDecimal:F2} лв";
    public string FormattedDateTime => OrderDateTime.ToString("dd.MM.yyyy HH:mm");
    public int ItemsCount => Items?.Count ?? 0;
    public string ShortStatus => Status switch
    {
        "pending" => "⏳ Чакаща",
        "ready_for_pickup" => "✅ Готова",
        "on_delivery" => "🚚 В доставка",
        "delivered" => "📦 Доставена",
        _ => Status
    };

    private string GetStatusDisplayName(string status)
    {
        return status switch
        {
            "pending" => "Чакаща",
            "ready_for_pickup" => "Готова за вземане",
            "on_delivery" => "В доставка",
            "delivered" => "Доставена",
            _ => status
        };
    }
}


// За създаване на поръчка
public class CreateOrderDto
{
    [JsonPropertyName("address_for_delivery")]
    public string AddressForDelivery { get; set; } = string.Empty;

    [JsonPropertyName("delivery_time")]
    public DateTime? DeliveryTime { get; set; }
}

// За детайли на поръчка (може да съдържа повече информация)
public class OrderDetailDto : OrderDto
{
    [JsonPropertyName("user_phone")]
    public string UserPhone { get; set; } = string.Empty;

    [JsonPropertyName("user_name")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("restaurant_phone")]
    public string? RestaurantPhone { get; set; }

    [JsonPropertyName("supplier_phone")]
    public string? SupplierPhone { get; set; }
}

// За промяна на статус
public class UpdateOrderStatusDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}