using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.User;

public class CartViewModel : BaseViewModel
{
    private readonly ICartService _cartService;
    private readonly INavigationService _navigationService;

    private CartDto _cart = new();
    private int _userProfileId;
    private bool _isRefreshing;

    public CartViewModel(
        ICartService cartService,
        INavigationService navigationService)
    {
        _cartService = cartService;
        _navigationService = navigationService;

        GoToHomeCommand = new Command(async () => await _navigationService.GoToAsync("user_home"));
        GoToProfileCommand = new Command(async () => await _navigationService.GoToAsync("user/profile"));
        GoToOrdersCommand = new Command(async () => await _navigationService.GoToAsync("my-orders"));
        LogoutCommand = new Command(async () => await ExecuteLogoutAsync());
        RemoveItemCommand = new Command<CartItemDto>(async (item) => await ExecuteRemoveItemAsync(item));
        CheckoutCommand = new Command(async () => await ExecuteCheckoutAsync());

        Task.Run(LoadCartAsync);
    }

    public CartDto Cart
    {
        get => _cart;
        set
        {
            _cart = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SubtotalText));
            OnPropertyChanged(nameof(DeliveryFeeText));
            OnPropertyChanged(nameof(TotalText));
            OnPropertyChanged(nameof(ShowDeliveryFee));

            (CheckoutCommand as Command)?.ChangeCanExecute();
        }
    }

    public int UserProfileId
    {
        get => _userProfileId;
        set
        {
            _userProfileId = value;
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

    // Helper properties за UI
    public string SubtotalText => $"Сума на продуктите: {Cart.SubtotalDecimal:F2} лв.";
    public string DeliveryFeeText => $"Такса доставка: {Cart.DeliveryFeeDecimal:F2} лв.";
    public string TotalText => $"Общо: {Cart.TotalDecimal:F2} лв.";
    public bool ShowDeliveryFee => Cart.DeliveryFeeDecimal > 0;

    public ICommand GoToHomeCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand GoToProfileCommand { get; }
    public ICommand GoToOrdersCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand CheckoutCommand { get; }

    public async Task LoadCartAsync()
    {
        try
        {
            IsBusy = true;
            System.Diagnostics.Debug.WriteLine("👉 LoadCartAsync започва...");

            var response = await _cartService.GetCartAsync();

            if (response != null)
            {
                UserProfileId = response.UserProfileId;
                Cart = response.Cart ?? new CartDto();  // ← Ако Cart е null, създай нов

                System.Diagnostics.Debug.WriteLine($"✅ Количката заредена. Артикули: {Cart.Items?.Count ?? 0}");

                // 🔥 Логвай всеки артикул
                if (Cart.Items != null && Cart.Items.Any())
                {
                    foreach (var item in Cart.Items)
                    {
                        System.Diagnostics.Debug.WriteLine($"   - {item.ArticleName} (ID: {item.Id}, Кол: {item.Quantity})");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при зареждане на количката: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExecuteRemoveItemAsync(CartItemDto item)
    {
        if (item == null) return;

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine($"👉 Премахване на артикул: {item.ArticleName} (ID: {item.Id})");

            var updatedCart = await _cartService.RemoveFromCartAsync(item.Id);

            if (updatedCart != null)
            {
                // 🔥 Обнови само Cart, не прави нов запрос
                Cart = updatedCart;

                System.Diagnostics.Debug.WriteLine($"✅ Артикулът премахнат. Остават {Cart.Items?.Count ?? 0} артикула");
            }
        }, "Грешка при премахване на артикул");
    }

    private async Task ExecuteCheckoutAsync()
    {
        if (Cart.IsEmpty) return;

        await _navigationService.GoToAsync("checkout");
    }

    private async Task ExecuteLogoutAsync()
    {
        // Логика за logout (ще я добавим после)
    }
}