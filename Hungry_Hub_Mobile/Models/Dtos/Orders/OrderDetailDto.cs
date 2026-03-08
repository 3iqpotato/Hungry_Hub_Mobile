using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Orders;

public class OrderDetailResponseDto
{
    [JsonPropertyName("order")]
    public OrderDto Order { get; set; } = new();

    [JsonPropertyName("can_update")]
    public bool CanUpdate { get; set; }

    [JsonPropertyName("can_pickup")]
    public bool CanPickup { get; set; }
}