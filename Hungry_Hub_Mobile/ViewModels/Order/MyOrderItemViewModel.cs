using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hungry_Hub_Mobile.Core.DTOs.Orders;

namespace Hungry_Hub_Mobile.ViewModels.Orders;

public class MyOrderItemViewModel
{
    public int Id { get; set; }
    public string? Status { get; set; }
    public decimal TotalPriceDecimal { get; set; }
    public decimal DeliveryFeeDecimal { get; set; }
    public DateTime OrderDateTime { get; set; }

    public int? RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
    public string? RestaurantAddress { get; set; }
    public string? RestaurantPhone { get; set; }

    public List<OrderItemDto>? Items { get; set; }

    public string StatusDisplayName => Status switch
    {
        "pending" => "Чакаща",
        "ready_for_pickup" => "Готова за вземане",
        "on_delivery" => "В доставка",
        "delivered" => "Доставена",
        _ => Status ?? "Неизвестен"
    };

    public string FormattedTotal => $"{TotalPriceDecimal:F2} лв";
    public string FormattedDeliveryFee => $"{DeliveryFeeDecimal:F2} лв";
    public string FormattedDateTime => OrderDateTime.ToString("dd.MM.yyyy HH:mm");
    public int ItemsCount => Items?.Count ?? 0;

    public bool HasRestaurant => RestaurantId != null;

    public string DisplayRestaurantAddress =>
        !string.IsNullOrWhiteSpace(RestaurantAddress) ? RestaurantAddress : "Няма адрес";

    public string DisplayRestaurantPhone =>
        !string.IsNullOrWhiteSpace(RestaurantPhone) ? RestaurantPhone : "Няма телефон";

    public static MyOrderItemViewModel FromDto(OrderDto dto)
    {
        return new MyOrderItemViewModel
        {
            Id = dto.Id,
            Status = dto.Status,
            TotalPriceDecimal = dto.TotalPriceDecimal,
            DeliveryFeeDecimal = dto.DeliveryFeeDecimal,
            OrderDateTime = dto.OrderDateTime,
            RestaurantId = dto.RestaurantId,
            RestaurantName = dto.RestaurantName,
            RestaurantAddress = dto.RestaurantAddress,
            RestaurantPhone = dto.RestaurantPhone,
            Items = dto.Items
        };
    }
}