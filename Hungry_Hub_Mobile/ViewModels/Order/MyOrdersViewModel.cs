using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hungry_Hub_Mobile.ViewModels.Orders;

public class MyOrdersViewModel : BaseViewModel
{
    private readonly IOrderService _orderService;
    private readonly INavigationService _navigationService;
    private readonly IAuthService _authService;

    private ObservableCollection<MyOrderItemViewModel> _orders = new();
    private bool _isRefreshing;
    private MyOrderItemViewModel? _selectedOrder;

    public MyOrdersViewModel(
        IOrderService orderService,
        INavigationService navigationService,
        IAuthService authService) : base(authService, navigationService)
    {
        _orderService = orderService;
        _navigationService = navigationService;
        _authService = authService;

        // Навигационни команди
        GoToHomeCommand = new Command(async () => await _navigationService.GoToAsync("user_home"));
        GoToCartCommand = new Command(async () => await _navigationService.GoToAsync("cart"));
        GoToOrdersCommand = new Command(async () => await _navigationService.GoToAsync("my-orders"));
        GoToProfileCommand = new Command(async () => await _navigationService.GoToAsync("user/profile"));
        //LogoutCommand = new Command(async () => await ExecuteLogoutAsync());

        // Съществуващи команди
        LoadOrdersCommand = new Command(async () => await LoadOrdersAsync());
        RefreshCommand = new Command(async () => await LoadOrdersAsync());
        SelectOrderCommand = new Command<MyOrderItemViewModel>(async (order) => await ExecuteSelectOrderAsync(order));
        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());

        // Зареди поръчките
        //Task.Run(LoadOrdersAsync);
    }

    public ObservableCollection<MyOrderItemViewModel> Orders
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

    public MyOrderItemViewModel? SelectedOrder
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

    // Навигационни команди
    public ICommand GoToHomeCommand { get; }
    public ICommand GoToCartCommand { get; }
    public ICommand GoToOrdersCommand { get; }
    public ICommand GoToProfileCommand { get; }
    //public ICommand LogoutCommand { get; }

    // Съществуващи команди
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
                var items = orders.Select(MyOrderItemViewModel.FromDto).ToList();
                Orders = new ObservableCollection<MyOrderItemViewModel>(items);

                System.Diagnostics.Debug.WriteLine($"✅ Заредени {Orders.Count} поръчки");
            }
            else
            {
                Orders = new ObservableCollection<MyOrderItemViewModel>();
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

    private async Task ExecuteSelectOrderAsync(MyOrderItemViewModel? order)
    {
        if (order == null) return;

        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Избрана поръчка #{order.Id}");


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

    //private async Task ExecuteLogoutAsync()
    //{
    //    await ExecuteAsync(async () =>
    //    {
    //        System.Diagnostics.Debug.WriteLine("👉 Logout от поръчките...");

    //        await _authService.LogoutAsync();
    //        await _navigationService.GoToAsync("start");

    //        System.Diagnostics.Debug.WriteLine("✅ Успешен logout от поръчките");
    //    }, "Грешка при изход");
    //}
}