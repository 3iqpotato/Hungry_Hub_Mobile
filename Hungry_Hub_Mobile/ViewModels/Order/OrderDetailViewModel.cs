using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.Orders;

[QueryProperty(nameof(OrderId), "orderId")]
public class OrderDetailViewModel : BaseViewModel
{
    private readonly IOrderService _orderService;
    private readonly INavigationService _navigationService;

    private int _orderId;
    private OrderDto _order = new();
    private bool _canUpdate;
    private bool _canPickup;
    private bool _isRefreshing;

    public OrderDetailViewModel(IOrderService orderService, INavigationService navigationService)
    {
        _orderService = orderService;
        _navigationService = navigationService;

        RefreshCommand = new Command(async () => await LoadOrderDetailAsync());
        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());
    }
    public int OrderId
    {
        get => _orderId;
        set
        {
            _orderId = value;
            OnPropertyChanged();
            System.Diagnostics.Debug.WriteLine($"👉 OrderDetailViewModel получи OrderId: {value}");

            // Зареди детайлите веднага щом получим ID
            if (value > 0)
            {
                Task.Run(LoadOrderDetailAsync);
            }
        }
    }

    public OrderDto Order
    {
        get => _order;
        set
        {
            _order = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasOrder));
            OnPropertyChanged(nameof(FormattedOrderDate));
            OnPropertyChanged(nameof(FormattedDeliveryDate));
            OnPropertyChanged(nameof(FormattedDeliveryFee));
            OnPropertyChanged(nameof(FormattedTotal));
            OnPropertyChanged(nameof(StatusColor));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(HasRestaurant));
            OnPropertyChanged(nameof(RestaurantAddress));
            OnPropertyChanged(nameof(RestaurantPhone));
            OnPropertyChanged(nameof(ItemsCount));
        }
    }

    public bool CanUpdate
    {
        get => _canUpdate;
        set
        {
            _canUpdate = value;
            OnPropertyChanged();
        }
    }

    public bool CanPickup
    {
        get => _canPickup;
        set
        {
            _canPickup = value;
            OnPropertyChanged();
        }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged();
        }
    }

    // Helper properties
    public bool HasOrder => Order != null && Order.Id > 0;
    public string FormattedOrderDate => Order?.OrderDateTime.ToString("dd MMM yyyy HH:mm") ?? "";
    public string FormattedDeliveryDate => Order?.DeliveryTime?.ToString("dd MMM yyyy HH:mm") ?? "Не е зададено";
    public string FormattedDeliveryFee => $"{Order?.DeliveryFeeDecimal:F2} лв";
    public string FormattedTotal => $"{Order?.TotalPriceDecimal:F2} лв";
    public Color StatusColor => GetStatusColor(Order?.Status);
    public string StatusText => GetStatusText(Order?.Status);
    public bool HasRestaurant => Order?.RestaurantId != null;
    public string RestaurantAddress => Order?.RestaurantName ?? "Не е наличен";
    public string RestaurantPhone => Order?.RestaurantName ?? "Не е наличен"; // Ако нямаме телефон, ползваме име
    public int ItemsCount => Order?.Items?.Count ?? 0;

    public ICommand RefreshCommand { get; }
    public ICommand GoBackCommand { get; }

    private async Task LoadOrderDetailAsync()
    {
        if (OrderId <= 0) return;

        try
        {
            IsRefreshing = true;
            ErrorMessage = string.Empty;

            System.Diagnostics.Debug.WriteLine($"👉 Зареждане на детайли за поръчка {OrderId}");

            var response = await _orderService.GetOrderDetailAsync(OrderId);

            if (response != null)
            {
                Order = response.Order;
                CanUpdate = response.CanUpdate;
                CanPickup = response.CanPickup;

                System.Diagnostics.Debug.WriteLine($"✅ Детайли заредени. Статус: {Order?.Status}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при зареждане на поръчка: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private Color GetStatusColor(string? status)
    {
        return status switch
        {
            "pending" => Colors.Orange,
            "ready_for_pickup" => Colors.Blue,
            "on_delivery" => Colors.Purple,
            "delivered" => Colors.Green,
            _ => Colors.Gray
        };
    }

    private string GetStatusText(string? status)
    {
        return status switch
        {
            "pending" => "В процес на приготвяне",
            "ready_for_pickup" => "Готова за вземане",
            "on_delivery" => "В доставка",
            "delivered" => "Доставена",
            _ => status ?? "Неизвестен статус"
        };
    }
}