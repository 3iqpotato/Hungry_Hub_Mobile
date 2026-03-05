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

    // За цветове в UI
    //public static Colors GetStatusColor(string status)
    //{
    //    return status switch
    //    {
    //        Pending => Colors.Orange,
    //        ReadyForPickup => Colors.Blue,
    //        OnDelivery => Colors.Purple,
    //        Delivered => Colors.Green,
    //        _ => Colors.Gray
    //    };
    //}
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
    public decimal Price { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("total_price")]
    public decimal TotalPrice { get; set; }
}

// За поръчка - съответства на OrderSerializer
public class OrderDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = OrderStatus.Pending;

    [JsonPropertyName("address_for_delivery")]
    public string AddressForDelivery { get; set; } = string.Empty;

    [JsonPropertyName("order_date_time")]
    public DateTime OrderDateTime { get; set; }

    [JsonPropertyName("delivery_time")]
    public DateTime? DeliveryTime { get; set; }  // Може да е null

    [JsonPropertyName("delivery_fee")]
    public decimal DeliveryFee { get; set; }

    [JsonPropertyName("total_price")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("restaurant")]
    public int? RestaurantId { get; set; }  // Може да е null (SET_NULL)

    [JsonPropertyName("restaurant_name")]
    public string? RestaurantName { get; set; }  // Може да е null

    [JsonPropertyName("supplier")]
    public int? SupplierId { get; set; }  // Може да е null

    [JsonPropertyName("supplier_name")]
    public string? SupplierName { get; set; }  // Може да е null

    [JsonPropertyName("items")]
    public List<OrderItemDto> Items { get; set; } = new();

    // Helper properties за UI
    public string StatusDisplayName => OrderStatus.GetDisplayName(Status);
    //public Color StatusColor => OrderStatus.GetStatusColor(Status);
    public string FormattedTotal => TotalPrice.ToString("F2") + " лв.";
    public string FormattedDateTime => OrderDateTime.ToString("dd.MM.yyyy HH:mm");
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