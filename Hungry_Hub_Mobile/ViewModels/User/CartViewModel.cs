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

    // Helper properties за UI
    public string SubtotalText => $"Сума на продуктите: {Cart.SubtotalDecimal:F2} лв.";
    public string DeliveryFeeText => $"Такса доставка: {Cart.DeliveryFeeDecimal:F2} лв.";
    public string TotalText => $"Общо: {Cart.TotalDecimal:F2} лв.";
    public bool ShowDeliveryFee => Cart.DeliveryFeeDecimal > 0;

    public ICommand GoToHomeCommand { get; }
    public ICommand GoToProfileCommand { get; }
    public ICommand GoToOrdersCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand CheckoutCommand { get; }

    private async Task LoadCartAsync()
    {
        try
        {
            IsBusy = true;

            var response = await _cartService.GetCartAsync();  // ← Това вече връща CartResponseDto

            if (response != null)
            {
                UserProfileId = response.UserProfileId;  // ← Това идва от CartResponseDto
                Cart = response.Cart;                    // ← Това идва от CartResponseDto.Cart

                System.Diagnostics.Debug.WriteLine($"✅ Количката заредена. {Cart.Items.Count} артикула");
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

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Потвърждение",
            $"Премахване на {item.ArticleName} от количката?",
            "Да", "Не");

        if (!confirm) return;

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine($"👉 Премахване: {item.ArticleName}");

            var response = await _cartService.RemoveFromCartAsync(item.ArticleId);

            if (response?.Cart != null)
            {
                Cart = response.Cart;
                System.Diagnostics.Debug.WriteLine($"✅ Премахнато. Остават {Cart.Items.Count} артикула");
            }
        }, "Грешка при премахване");
    }

    private async Task ExecuteCheckoutAsync()
    {
        if (Cart.IsEmpty)
        {
            await Application.Current.MainPage.DisplayAlert("Количката е празна",
                "Добавете продукти, преди да поръчате.", "OK");
            return;
        }

        // Тук после ще добавим навигация към страница за поръчка
        await Application.Current.MainPage.DisplayAlert("Поръчка",
            "Функционалността за поръчка предстои", "OK");
    }

    private async Task ExecuteLogoutAsync()
    {
        // Логика за logout (ще я добавим после)
    }
}