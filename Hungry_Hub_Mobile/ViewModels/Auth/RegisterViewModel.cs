using System.Text.RegularExpressions;
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
        ErrorMessage = string.Empty;

        var validationErrors = ValidateRegisterForm();

        if (validationErrors.Any())
        {
            ErrorMessage = string.Join(Environment.NewLine, validationErrors);
            return;
        }

        var request = new RegisterRequestDto
        {
            Email = Email.Trim(),
            Password = Password,
            Type = SelectedUserType
        };

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine($"👉 Опит за регистрация: {Email}, тип: {SelectedUserType}");

            var response = await _authService.RegisterAsync(request);

            if (response?.User != null)
            {
                System.Diagnostics.Debug.WriteLine("✅ Успешна регистрация!");
                System.Diagnostics.Debug.WriteLine($"👉 Next route: {response.Next}");

                if (response.ProfileId.HasValue)
                {
                    await TokenStorage.SaveProfileIdAsync(response.ProfileId.Value);
                }

                if (!string.IsNullOrEmpty(response.Next))
                {
                    await _navigationService.GoToAsync(response.Next);
                }
                else
                {
                    await _navigationService.GoToAsync("///start");
                }
            }
            else
            {
                ErrorMessage = "Невалиден отговор от сървъра.";
            }
        }, "Грешка при регистрация");
    }

    private List<string> ValidateRegisterForm()
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
        else
        {
            if (Password.Length < 6)
            {
                errors.Add("• Паролата трябва да е поне 6 символа.");
            }

            if (!Password.Any(char.IsUpper))
            {
                errors.Add("• Паролата трябва да съдържа поне една главна буква.");
            }

            if (!Password.Any(char.IsDigit))
            {
                errors.Add("• Паролата трябва да съдържа поне една цифра.");
            }
        }

        if (string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            errors.Add("• Полето за потвърждение на парола е задължително.");
        }
        else if (Password != ConfirmPassword)
        {
            errors.Add("• Паролите не съвпадат.");
        }

        if (string.IsNullOrWhiteSpace(SelectedUserType))
        {
            errors.Add("• Моля изберете тип потребител.");
        }

        return errors;
    }

    private bool IsValidGmail(string email)
    {
        return Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@gmail\.com$");
    }
}