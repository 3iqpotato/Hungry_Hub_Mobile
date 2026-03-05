using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Auth;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.Auth;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    private string _email;
    private string _password;

    public LoginViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;

        LoginCommand = new Command(async () => await ExecuteLoginAsync());
        GoToRegisterCommand = new Command(async () => await ExecuteGoToRegisterAsync());
    }

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    private async Task ExecuteLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Моля попълнете email и парола";
            return;
        }

        var request = new LoginRequestDto
        {
            Email = Email,
            Password = Password
        };

        await ExecuteAsync(async () =>
        {
            var response = await _authService.LoginAsync(request);

            Console.WriteLine(response);

            if (response?.User != null)
            {
                // Успешен вход - пренасочваме според типа потребител
                if (response.User.Type == "user")

                {
                    System.Diagnostics.Debug.WriteLine($"👉 Отиваме към: {response.Next}");
                    await _navigationService.GoToAsync(response.Next);
                }
                else if (response.User.Type == "supplier")
                    await _navigationService.GoToAsync("///supplier/home");
                else if (response.User.Type == "restaurant")
                    await _navigationService.GoToAsync("///restaurant/home");
            }
            else
            {
                ErrorMessage = "Невалиден отговор от сървъра";
            }
        }, "Грешка при вход. Проверете email и парола.");
    }

    private async Task ExecuteGoToRegisterAsync()
    {
        await _navigationService.GoToAsync("register");
    }
}