// Core/DTOs/Suppliers/SupplierDto.cs
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using System.Text.Json.Serialization;

namespace Hungry_Hub_Mobile.Core.DTOs.Suppliers;

// За типа транспорт - съответства на TRANSPORT_CHOICES
using System.Text.Json.Serialization;

// Статичен клас за транспортните типове
public class TransportTypeItem
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public override string ToString() => DisplayName;
}

// За GET заявка - какво връща API-то
public class SupplierProfileResponseDto
{
    [JsonPropertyName("profile_exists")]
    public bool ProfileExists { get; set; }

    [JsonPropertyName("supplier")]
    public SupplierDto? Supplier { get; set; }
}

// Основен Supplier DTO
public class SupplierDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("img")]
    public string? Img { get; set; }

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string? Type { get; set; }  // 'car', 'motorcycle', etc.

    [JsonPropertyName("daily_earnings")]
    public string DailyEarnings { get; set; } = "0.00";  // Идва като string

    [JsonPropertyName("last_reset")]
    public DateTime LastReset { get; set; }


    [JsonIgnore]
    public decimal DailyEarningsDecimal
    {
        get
        {
            if (decimal.TryParse(DailyEarnings, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            return 0;
        }
    }
}

// За POST заявка - какво връща API-то след запис
public class CompleteSupplierResponseDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("supplier")]
    public SupplierDto Supplier { get; set; } = new();

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;
}

// За изпращане към API-то
public class UpdateSupplierProfileDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("img")]
    public string? Img { get; set; }

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

// За home page на supplier
public class SupplierHomeDto
{
    [JsonPropertyName("profile")]
    public SupplierDto Profile { get; set; } = new();
    
    [JsonPropertyName("available_orders")]
    public List<AvailableOrderDto>? AvailableOrders { get; set; } = new();
    
    [JsonPropertyName("active_orders")]
    public List<OrderDto>? ActiveOrders { get; set; } = new();
    
    [JsonPropertyName("today_earnings")]
    public decimal? TodayEarnings { get; set; }
    
    [JsonPropertyName("monthly_earnings")]
    public decimal? MonthlyEarnings { get; set; }
}

// За available order (опростен вариант за списъка с налични поръчки)
public class AvailableOrderDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("restaurant_name")]
    public string? RestaurantName { get; set; } = string.Empty;
    
    [JsonPropertyName("address_for_delivery")]
    public string? AddressForDelivery { get; set; } = string.Empty;
    
    [JsonPropertyName("delivery_fee")]
    public decimal? DeliveryFee { get; set; }
    
    [JsonPropertyName("total_price")]
    public decimal? TotalPrice { get; set; }
    
    [JsonPropertyName("items_count")]
    public int? ItemsCount { get; set; }
}