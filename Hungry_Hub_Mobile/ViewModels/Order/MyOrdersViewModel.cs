using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.Orders;

public class MyOrdersViewModel : BaseViewModel
{
    private readonly IOrderService _orderService;
    private readonly INavigationService _navigationService;

    private List<OrderDto> _orders = new();
    private bool _isRefreshing;
    private OrderDto? _selectedOrder;

    public MyOrdersViewModel(IOrderService orderService, INavigationService navigationService)
    {
        _orderService = orderService;
        _navigationService = navigationService;

        LoadOrdersCommand = new Command(async () => await LoadOrdersAsync());
        RefreshCommand = new Command(async () => await LoadOrdersAsync());
        SelectOrderCommand = new Command<OrderDto>(async (order) => await ExecuteSelectOrderAsync(order));
        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());

        Task.Run(LoadOrdersAsync);
    }

    public List<OrderDto> Orders
    {
        get => _orders;
        set
        {
            _orders = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasOrders));
            OnPropertyChanged(nameof(EmptyMessage));
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

    public OrderDto? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
        }
    }

    public bool HasOrders => Orders.Any();
    public string EmptyMessage => "Все още нямате поръчки";

    public ICommand LoadOrdersCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SelectOrderCommand { get; }
    public ICommand GoBackCommand { get; }

    private async Task LoadOrdersAsync()
    {
        try
        {
            IsRefreshing = true;
            ErrorMessage = string.Empty;

            System.Diagnostics.Debug.WriteLine("👉 Зареждане на поръчки...");

            var orders = await _orderService.GetMyOrdersAsync();

            if (orders != null)
            {
                Orders = orders;
                System.Diagnostics.Debug.WriteLine($"✅ Заредени {orders.Count} поръчки");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при зареждане на поръчки: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task ExecuteSelectOrderAsync(OrderDto? order)
    {
        if (order == null) return;

        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Избрана поръчка #{order.Id}");

            SelectedOrder = order;

            // 🔥 Използвай Dictionary за параметри
            var parameters = new Dictionary<string, object>
        {
            { "orderId", order.Id }
        };

            await _navigationService.GoToAsync("order_detail", parameters);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при отваряне на поръчка: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex}");
        }
    }
}