using Hungry_Hub_Mobile.Core.DTOs.Articles;
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using Hungry_Hub_Mobile.Services.Interfaces;
using System.Windows.Input;

namespace Hungry_Hub_Mobile.ViewModels.User;

public class RestaurantDetailViewModel : BaseViewModel
{
    private readonly IRestaurantMenuService _menuService;
    private readonly INavigationService _navigationService;
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;  // ← ДОБАВИ

    private RestaurantMiniDto _restaurant = new();
    private MenuDto _menu = new();
    private List<ArticleDto> _articles = new();
    private string _selectedFilter = "Всички";  // ← инициализирай с стойност
    private int _restaurantId;

    // Списък с налични филтри
    public List<string> FoodFilters { get; } = new()
    {
        "Всички",
        "salads",
        "appetizers",
        "main_course",
        "desserts"
    };

    // За визуализация на български
    public Dictionary<string, string> FilterDisplayNames { get; } = new()
    {
        ["Всички"] = "Всички",
        ["salads"] = "Салати",
        ["appetizers"] = "Предястия",
        ["main_course"] = "Основни",
        ["desserts"] = "Десерти"
    };

    public RestaurantDetailViewModel(
        IRestaurantMenuService menuService,
        INavigationService navigationService,
        ICartService cartService,
        IAuthService authService) : base(authService, navigationService)   // ← ДОБАВИ IAuthService
    {
        _menuService = menuService;
        _navigationService = navigationService;
        _cartService = cartService;
        _authService = authService;

        // 🔥 КОМАНДИ ЗА НАВИГАЦИЯ (долно меню)
        GoToHomeCommand = new Command(async () => await _navigationService.GoToAsync("user_home"));
        GoToCartCommand = new Command(async () => await _navigationService.GoToAsync("cart"));
        GoToOrdersCommand = new Command(async () => await _navigationService.GoToAsync("my-orders"));
        GoToProfileCommand = new Command(async () => await _navigationService.GoToAsync("user/profile"));
        LogoutCommand = new Command(async () => await ExecuteLogoutAsync());

        // 🔥 СЪЩЕСТВУВАЩИ КОМАНДИ
        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());
        FilterChangedCommand = new Command<string>(async (filter) => await ApplyFilterAsync(filter));
        SelectArticleCommand = new Command<ArticleDto>(async (article) => await ExecuteSelectArticleAsync(article));
        AddToCartCommand = new Command<ArticleDto>(async (article) => await ExecuteAddToCartAsync(article));
    }

    public RestaurantMiniDto Restaurant
    {
        get => _restaurant;
        set
        {
            _restaurant = value;
            OnPropertyChanged();
        }
    }

    public MenuDto Menu
    {
        get => _menu;
        set
        {
            _menu = value;
            OnPropertyChanged();
        }
    }

    public List<ArticleDto> Articles
    {
        get => _articles;
        set
        {
            _articles = value;
            OnPropertyChanged();
        }
    }

    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            _selectedFilter = value;
            OnPropertyChanged();
        }
    }

    // 🔥 НОВИ КОМАНДИ ЗА НАВИГАЦИЯ
    public ICommand GoToHomeCommand { get; }
    public ICommand GoToCartCommand { get; }
    public ICommand GoToOrdersCommand { get; }
    public ICommand GoToProfileCommand { get; }
    public ICommand LogoutCommand { get; }

    // 🔥 СЪЩЕСТВУВАЩИ КОМАНДИ
    public ICommand GoBackCommand { get; }
    public ICommand FilterChangedCommand { get; }
    public ICommand SelectArticleCommand { get; }
    public ICommand AddToCartCommand { get; }

    // Метод за инициализация с параметри
    public async Task InitializeAsync(int restaurantId)
    {
        _restaurantId = restaurantId;
        SelectedFilter = "Всички";
        await LoadMenuAsync();
    }

    private async Task ExecuteAddToCartAsync(ArticleDto article)
    {
        if (article == null) return;

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine($"👉 Добавяне в количка: {article.Name} (ID: {article.Id})");

            var response = await _cartService.AddToCartAsync(article.Id);

            if (response?.Success == true)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Добавено",
                    $"{article.Name} е добавен в количката",
                    "OK");

                System.Diagnostics.Debug.WriteLine($"✅ Добавено. Сега в количката: {response.Cart?.Items?.Count} артикула");
            }
            else
            {
                ErrorMessage = "Грешка при добавяне в количката";
            }
        }, "Грешка при добавяне в количката");
    }

    private async Task LoadMenuAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            string? filterType = SelectedFilter == "Всички" ? null : SelectedFilter;

            var data = await _menuService.GetMenuForUsersAsync(_restaurantId, filterType);

            if (data != null)
            {
                Restaurant = data.Restaurant;
                Menu = data.Menu;
                Articles = data.Articles;

                System.Diagnostics.Debug.WriteLine($"✅ Заредени {Articles.Count} артикула");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при зареждане на менюто: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ApplyFilterAsync(string filter)
    {
        if (SelectedFilter != filter)
        {
            SelectedFilter = filter;
            await LoadMenuAsync();
        }
    }

    private async Task ExecuteSelectArticleAsync(ArticleDto article)
    {
        if (article != null)
        {
            System.Diagnostics.Debug.WriteLine($"👉 Избран артикул: {article.Name}");

            var parameters = new Dictionary<string, object>
            {
                { "articleId", article.Id },
                { "restaurantId", _restaurantId }
            };
            // await _navigationService.GoToAsync("article/details", parameters);
        }
    }

    private async Task ExecuteLogoutAsync()
    {
        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Logout от детайли на ресторант...");

            if (_authService != null)
                await _authService.LogoutAsync();

            await _navigationService.GoToAsync("start");
        }, "Грешка при изход");
    }
}