using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Auth;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.Auth;

public class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    private string _email;
    private string _password;
    private string _confirmPassword;
    private string _selectedUserType;
    private bool _isRegistrationSuccessful;

    // Списък с типове потребители за падащото меню
    public List<string> UserTypes { get; } = new()
    {
        "user",
        "supplier",
        "restaurant"
    };

    public RegisterViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;

        RegisterCommand = new Command(async () => await ExecuteRegisterAsync());
        GoToLoginCommand = new Command(async () => await _navigationService.GoToAsync("login"));
    }
    [StringLength(30,MinimumLength = 5)]
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

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            _confirmPassword = value;
            OnPropertyChanged();
        }
    }

    public string SelectedUserType
    {
        get => _selectedUserType;
        set
        {
            _selectedUserType = value;
            OnPropertyChanged();
        }
    }

    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    private async Task ExecuteRegisterAsync()
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Моля попълнете всички полета";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Паролите не съвпадат";
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedUserType))
        {
            ErrorMessage = "Моля изберете тип потребител";
            return;
        }

        var request = new RegisterRequestDto
        {
            Email = Email,
            Password = Password,
            Type = SelectedUserType
        };

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine($"👉 Опит за регистрация: {Email}, тип: {SelectedUserType}");

            var response = await _authService.RegisterAsync(request);

            if (response?.User != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Успешна регистрация!");
                System.Diagnostics.Debug.WriteLine($"👉 Next route: {response.Next}");

                if (response.ProfileId.HasValue)
                {
                    await TokenStorage.SaveProfileIdAsync(response.ProfileId.Value);
                }

                // Пренасочване според next параметъра
                if (!string.IsNullOrEmpty(response.Next))
                {
                    await _navigationService.GoToAsync(response.Next);
                }
                else
                {
                    // Ако няма next, отиваме на StartPage
                    await _navigationService.GoToAsync("///start");
                }
            }
            else
            {
                ErrorMessage = "Невалиден отговор от сървъра";
            }
        }, "Грешка при регистрация");
    }
}