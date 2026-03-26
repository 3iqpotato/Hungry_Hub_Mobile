using System.Text.RegularExpressions;
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
        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());
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

    public ICommand GoBackCommand { get; }
    private async Task ExecuteLoginAsync()
    {
        ErrorMessage = string.Empty;

        var validationErrors = ValidateLoginForm();

        if (validationErrors.Any())
        {
            ErrorMessage = string.Join(Environment.NewLine, validationErrors);
            return;
        }

        var request = new LoginRequestDto
        {
            Email = Email.Trim(),
            Password = Password
        };

        await ExecuteAsync(async () =>
        {
            var response = await _authService.LoginAsync(request);

            Console.WriteLine(response);

            if (response?.User != null)
            {
                if (response.User.Type == "user")
                {
                    System.Diagnostics.Debug.WriteLine($"👉 Отиваме към: {response.Next}");
                    await _navigationService.GoToAsync(response.Next);
                }
                else if (response.User.Type == "supplier")
                {
                    await _navigationService.GoToAsync("///supplier/home");
                }
                else if (response.User.Type == "restaurant")
                {
                    await _navigationService.GoToAsync("///restaurant/home");
                }
            }
            else
            {
                ErrorMessage = "Невалиден отговор от сървъра.";
            }
        }, "Грешка при вход. Проверете email и парола.");
    }

    private List<string> ValidateLoginForm()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Email))
        {
            errors.Add("• Полето за имейл е задължително.");
        }
        else
        {
            var email = Email.Trim();

            if (!IsValidGmail(email))
            {
                errors.Add("• Моля въведете валиден Gmail адрес.");
            }
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            errors.Add("• Полето за парола е задължително.");
        }

        return errors;
    }

    private bool IsValidGmail(string email)
    {
        return Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@gmail\.com$");
    }

    private async Task ExecuteGoToRegisterAsync()
    {
        await _navigationService.GoToAsync("register");
    }
}