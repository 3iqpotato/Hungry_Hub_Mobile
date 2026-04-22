using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.User;

public class UserHomeViewModel : BaseViewModel
{
    private readonly IUserHomeService _userHomeService;

    private readonly INavigationService _navigationService;

    private UserProfileDto _userProfile = new();
    private List<RestaurantMiniDto> _restaurants = new();
    private bool _isRefreshing;

    public UserHomeViewModel(
        IUserHomeService userHomeService,
        IAuthService authService,
        INavigationService navigationService) : base(authService, navigationService)
    {
        _userHomeService = userHomeService;
        _navigationService = navigationService;

        // Команди
        GoToHomeCommand = new Command(async () => await _navigationService.GoToAsync("user_home"));
        GoToCartCommand = new Command(async () => await _navigationService.GoToAsync("cart"));
        GoToOrdersCommand = new Command(async () => await _navigationService.GoToAsync("my-orders"));
        GoToProfileCommand = new Command(async () => await _navigationService.GoToAsync("user/profile"));
        //LogoutCommand = new Command(async () => await ExecuteLogoutAsync());
        RefreshCommand = new Command(async () => await LoadUserHomeAsync());
        SelectRestaurantCommand = new Command<RestaurantMiniDto>(async (restaurant) =>
            await ExecuteSelectRestaurantAsync(restaurant));

        // Зареди данните
        //Task.Run(LoadUserHomeAsync);
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
    public ICommand GoToHomeCommand { get; }
    public ICommand GoToCartCommand { get; }
    public ICommand GoToOrdersCommand { get; }
    public ICommand GoToProfileCommand { get; }
    //public ICommand LogoutCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SelectRestaurantCommand { get; }

    public async Task LoadUserHomeAsync()
    {
        try
        {
            IsRefreshing = true;
            var data = await _userHomeService.GetUserHomeAsync();
            if (data != null)
            {
                UserProfile = data.Profile;
                Restaurants = data.Restaurants;
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

    //private async Task ExecuteLogoutAsync()
    //{
    //    await ExecuteAsync(async () =>
    //    {
    //        await _authService.LogoutAsync();
    //        await _navigationService.GoToAsync("start");
    //    }, "Грешка при изход");
    //}

    private async Task ExecuteSelectRestaurantAsync(RestaurantMiniDto restaurant)
    {
        if (restaurant != null)
        {
            System.Diagnostics.Debug.WriteLine($"👉 Избран ресторант: {restaurant.Name} (ID: {restaurant.Id})");

            var parameters = new Dictionary<string, object>
            {
                { "restaurantId", restaurant.Id }
            };

            await _navigationService.GoToAsync("restaurant/details", parameters);
        }
    }
}