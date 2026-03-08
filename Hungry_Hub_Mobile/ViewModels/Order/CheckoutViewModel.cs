using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.Orders;

public class CheckoutViewModel : BaseViewModel
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IUserProfileService _userProfileService;
    private readonly INavigationService _navigationService;

    private CartDto _cart = new();
    private bool _isProcessing;
    private string _userAddress = string.Empty;

    public CheckoutViewModel(
        IOrderService orderService,
        ICartService cartService,
        IUserProfileService userProfileService,
        INavigationService navigationService)
    {
        _orderService = orderService;
        _cartService = cartService;
        _userProfileService = userProfileService;
        _navigationService = navigationService;

        PlaceOrderCommand = new Command(async () => await ExecutePlaceOrderAsync());
        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());

        // Зареди количката
        Task.Run(LoadCartAsync);
    }

    public CartDto Cart
    {
        get => _cart;
        set
        {
            _cart = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCartEmpty));
            OnPropertyChanged(nameof(FormattedSubtotal));
            OnPropertyChanged(nameof(FormattedDeliveryFee));
            OnPropertyChanged(nameof(FormattedTotal));
            OnPropertyChanged(nameof(HasDeliveryFee));
        }
    }

    public string UserAddress
    {
        get => _userAddress;
        set
        {
            _userAddress = value;
            OnPropertyChanged();
        }
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            _isProcessing = value;
            OnPropertyChanged();
        }
    }

    public bool IsCartEmpty => Cart?.IsEmpty ?? true;
    public string FormattedSubtotal => $"{Cart?.Subtotal:F2} лв";
    public string FormattedDeliveryFee => $"{Cart?.DeliveryFee:F2} лв";
    public string FormattedTotal => $"{Cart?.Total:F2} лв";
    public bool HasDeliveryFee => Cart?.DeliveryFeeDecimal > 0; // TODO moze da e problem 
    public ICommand PlaceOrderCommand { get; }
    public ICommand GoBackCommand { get; }


    private async Task LoadCartAsync()
    {
        try
        {
            IsBusy = true;

            var response = await _cartService.GetCartAsync();
            if (response != null)
            {
                Cart = response.Cart;
            }

            var profile = await _userProfileService.GetProfileAsync();
            if (profile != null)
            {
                UserAddress = profile.Address;
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

    private async Task ExecutePlaceOrderAsync()
    {
        if (IsCartEmpty)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Грешка",
                "Количката е празна",
                "OK");
            return;
        }

        // Потвърждение
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Потвърждение",
            $"Сигурни ли сте, че искате да направите поръчка за {FormattedTotal}?",
            "Да", "Не");

        if (!confirm) return;

        await ExecuteAsync(async () =>
        {
            IsProcessing = true;

            System.Diagnostics.Debug.WriteLine("👉 Изпращане на поръчка...");

            var response = await _orderService.CreateOrderAsync();

            if (response != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Поръчката създадена! ID: {response.OrderId}");

                // 🔥 Използвай MainThread за навигация
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        // Първо покажи съобщението
                        await Application.Current.MainPage.DisplayAlert(
                            "Успех",
                            $"Поръчката ви е създадена успешно! Номер: {response.OrderId}",
                            "OK");

                        // 🔥 МАЛКА ПАУЗА
                        await Task.Delay(100);

                        // 🔥 ПРОВЕРКА ЗА NULL
                        if (Shell.Current != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"👉 Навигация към order_detail?orderId={response.OrderId}");
                            await Shell.Current.GoToAsync($"order_detail?orderId={response.OrderId}");
                        }
                        else if (_navigationService != null)
                        {
                            System.Diagnostics.Debug.WriteLine("⚠️ Shell.Current е null, ползвам _navigationService");
                            var parameters = new Dictionary<string, object> { { "orderId", response.OrderId } };
                            await _navigationService.GoToAsync("order_detail", parameters);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("❌ Няма навигационен сервис!");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Грешка при навигация: {ex.Message}");
                    }
                });
            }

        }, "Грешка при създаване на поръчка");
    }
}