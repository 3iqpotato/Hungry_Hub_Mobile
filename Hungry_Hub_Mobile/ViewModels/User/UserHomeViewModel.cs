using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Auth;        // ← ДОБАВИ ТОВА за UserAccountDto
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.User;

public class UserHomeViewModel : BaseViewModel
{
    private readonly IUserHomeService _userHomeService;
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    private UserProfileDto _userProfile = new();
    private List<RestaurantMiniDto> _restaurants = new();
    private bool _isRefreshing;

    public UserHomeViewModel(
        IUserHomeService userHomeService,
        IAuthService authService,
        INavigationService navigationService)
    {
        _userHomeService = userHomeService;
        _authService = authService;
        _navigationService = navigationService;

        // Команди
        LogoutCommand = new Command(async () => await ExecuteLogoutAsync());
        GoToCartCommand = new Command(async () => await _navigationService.GoToAsync("cart"));
        GoToOrdersCommand = new Command(async () => await _navigationService.GoToAsync("my-orders"));
        // В конструктора - промени командата за профил
        GoToProfileCommand = new Command(async () => await _navigationService.GoToAsync("user/profile"));
        RefreshCommand = new Command(async () => await LoadUserHomeAsync());
        SelectRestaurantCommand = new Command<RestaurantMiniDto>(async (restaurant) =>
            await ExecuteSelectRestaurantAsync(restaurant));

        // Зареди данните
        Task.Run(LoadUserHomeAsync);
    }

    public UserProfileDto UserProfile
    {
        get => _userProfile;
        set
        {
            _userProfile = value;
            OnPropertyChanged();
        }
    }

    public List<RestaurantMiniDto> Restaurants
    {
        get => _restaurants;
        set
        {
            _restaurants = value;
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

    // Команди
    public ICommand LogoutCommand { get; }
    public ICommand GoToCartCommand { get; }
    public ICommand GoToOrdersCommand { get; }
    public ICommand GoToProfileCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SelectRestaurantCommand { get; }

    private async Task LoadUserHomeAsync()
    {
        try
        {
            IsRefreshing = true;

            // 🔥 МАХНАХМЕ user - не ни трябва
            // var user = await TokenStorage.GetUserAsync<UserAccountDto>();

            // 🔥 ИЗПОЛЗВАМЕ директно GetProfileIdAsync
            var profileId = await TokenStorage.GetProfileIdAsync();

            if (profileId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"👉 Зареждане на user home за profileId: {profileId.Value}");

                var data = await _userHomeService.GetUserHomeAsync(profileId.Value);

                if (data != null)
                {
                    UserProfile = data.Profile;
                    Restaurants = data.Restaurants;

                    System.Diagnostics.Debug.WriteLine($"✅ Заредени {Restaurants.Count} ресторанта");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Няма profile_id в storage!");
                ErrorMessage = "Грешка: Няма profile_id. Моля влезте отново.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при зареждане: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    // 🔥 МАХНАХМЕ GetProfileIdAsync - вече го има в TokenStorage

    private async Task ExecuteLogoutAsync()
    {
        await ExecuteAsync(async () =>
        {
            await _authService.LogoutAsync();
            await _navigationService.GoToAsync("///start");
        }, "Грешка при изход");
    }

    private async Task ExecuteSelectRestaurantAsync(RestaurantMiniDto restaurant)
    {
        if (restaurant != null)
        {
            System.Diagnostics.Debug.WriteLine($"👉 Избран ресторант: {restaurant.Name} (ID: {restaurant.Id})");

            // Подай restaurantId като параметър
            var parameters = new Dictionary<string, object>
        {
            { "restaurantId", restaurant.Id }
        };

            await _navigationService.GoToAsync("restaurant/details", parameters);
        }
    }
}