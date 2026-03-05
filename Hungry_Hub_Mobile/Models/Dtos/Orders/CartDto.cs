// Core/DTOs/Orders/CartDto.cs
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Orders;

// За CartItem - съответства на CartItemSerializer
public class CartItemDto
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

// За добавяне на артикул в кошницата
public class AddToCartDto
{
    [JsonPropertyName("article_id")]
    public int ArticleId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 1;
}

// За кошница - съответства на CartSerializer
public class CartDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("items")]
    public List<CartItemDto> Items { get; set; } = new();

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }

    [JsonPropertyName("delivery_fee")]
    public decimal DeliveryFee { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    // Helper property - дали кошницата е празна
    public bool IsEmpty => Items == null || Items.Count == 0;
}