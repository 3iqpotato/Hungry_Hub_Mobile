using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Auth;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels;

public class StartPageViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    private bool _isLoggedIn;
    private bool _isChecking = true;
    private string _welcomeMessage;

    public StartPageViewModel(IAuthService authService, INavigationService navigationService)
    {
        // Проверка дали услугите са инжектирани правилно
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        // Инициализиране на командите
        GoToLoginCommand = new Command(async () => await ExecuteGoToLogin());
        GoToRegisterCommand = new Command(async () => await ExecuteGoToRegister());
        GoToHomeCommand = new Command(async () => await NavigateToHome());
        LogoutCommand = new Command(async () => await ExecuteLogout());

        System.Diagnostics.Debug.WriteLine("🔥 StartPageViewModel constructor END");

        // Провери статуса при зареждане
        //Task.Run(CheckLoginStatus);
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            _isLoggedIn = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotLoggedIn));
        }
    }

    public bool IsNotLoggedIn => !_isLoggedIn;

    public bool IsChecking
    {
        get => _isChecking;
        set
        {
            _isChecking = value;
            OnPropertyChanged();
        }
    }

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set
        {
            _welcomeMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand GoToLoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }
    public ICommand GoToHomeCommand { get; }
    public ICommand LogoutCommand { get; }

    // 👇 ДОБАВЕНИ МЕТОДИ
    private async Task ExecuteGoToLogin()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Опит за навигация към login");

            if (_navigationService == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ _navigationService е null!");
                ErrorMessage = "Грешка в навигацията";
                return;
            }

            await _navigationService.GoToAsync("login");
            System.Diagnostics.Debug.WriteLine("✅ Успешна навигация към login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при навигация: {ex.Message}");
            ErrorMessage = $"Грешка: {ex.Message}";
        }
    }

    private async Task ExecuteGoToRegister()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Опит за навигация към register");

            if (_navigationService == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ _navigationService е null!");
                ErrorMessage = "Грешка в навигацията";
                return;
            }

            await _navigationService.GoToAsync("register");
            System.Diagnostics.Debug.WriteLine("✅ Успешна навигация към register");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при навигация: {ex.Message}");
            ErrorMessage = $"Грешка: {ex.Message}";
        }
    }
    public async Task RefreshLoginStatusAsync()
    {
        await CheckLoginStatus();
    }

    private async Task CheckLoginStatus()
    {
        try
        {
            IsChecking = true;
            System.Diagnostics.Debug.WriteLine("👉 Проверка на login статус...");

            var isAuthenticated = await _authService.IsAuthenticatedAsync();
            System.Diagnostics.Debug.WriteLine($"📊 isAuthenticated: {isAuthenticated}");

            if (isAuthenticated)
            {
                var userType = await TokenStorage.GetUserTypeAsync();
                var user = await TokenStorage.GetUserAsync<UserAccountDto>();

                System.Diagnostics.Debug.WriteLine($"📊 userType: {userType}");
                System.Diagnostics.Debug.WriteLine($"📊 user: {user?.Email}");

                WelcomeMessage = $"Добре дошли, {FormatDisplayName(user?.Email)}!";
                IsLoggedIn = true;
            }
            else
            {
                TokenStorage.RemoveTokens();
                IsLoggedIn = false;
                WelcomeMessage = string.Empty;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при проверка: {ex}");
            TokenStorage.RemoveTokens();
            IsLoggedIn = false;
        }
        finally
        {
            IsChecking = false;
        }
    }

    private async Task NavigateToHome()
    {
        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Навигация към home...");

            var hasProfile = await TokenStorage.HasCompleteProfileAsync();
            var userType = await TokenStorage.GetUserTypeAsync();
            var nextRoute = await TokenStorage.GetNextRouteAsync();

            System.Diagnostics.Debug.WriteLine($"📊 hasProfile: {hasProfile}");
            System.Diagnostics.Debug.WriteLine($"📊 userType: {userType}");
            System.Diagnostics.Debug.WriteLine($"📊 nextRoute: {nextRoute}");

            string route;

            if (!hasProfile && !string.IsNullOrEmpty(nextRoute))
            {
                route = nextRoute;
                System.Diagnostics.Debug.WriteLine($"👉 Отиваме към попълване на профил: {route}");
            }
            else
            {
                route = userType switch
                {
                    "user" => "user_home",
                    "supplier" => "supplier_home",
                    "restaurant" => "restaurant_home",
                    _ => "login"
                };
                System.Diagnostics.Debug.WriteLine($"👉 Отиваме към home: {route}");
            }

            await _navigationService.GoToAsync(route);
            System.Diagnostics.Debug.WriteLine($"✅ Успешна навигация към {route}");
        }, "Грешка при навигация към началната страница");
    }

    private async Task ExecuteLogout()
    {
        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Изпълнение на logout...");

            await _authService.LogoutAsync();

            IsLoggedIn = false;
            WelcomeMessage = string.Empty;

            await _navigationService.GoToAsync("///start");

            System.Diagnostics.Debug.WriteLine("✅ Успешен logout");
        }, "Грешка при изход");
    }

    private string FormatDisplayName(string email)
    {
        if (string.IsNullOrEmpty(email))
            return "потребител";

        var atIndex = email.IndexOf('@');
        if (atIndex > 0)
        {
            var username = email.Substring(0, atIndex);

            // Ако е твърде дълго, съкрати
            if (username.Length > 15)
                return username.Substring(0, 12) + "...";

            return username;
        }

        return email;
    }
}