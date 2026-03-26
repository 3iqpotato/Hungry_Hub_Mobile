using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.User;

public class CompleteUserProfileViewModel : BaseViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly ILocationService _locationService;
    private readonly INavigationService _navigationService;

    private string _name;
    private string _phoneNumber;
    private string _address;
    private string _profileImage;

    public CompleteUserProfileViewModel(
        IUserProfileService userProfileService,
        ILocationService locationService,
        INavigationService navigationService)
    {
        _userProfileService = userProfileService;
        _locationService = locationService;
        _navigationService = navigationService;

        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());
        SaveProfileCommand = new Command(async () => await ExecuteSaveProfileAsync());
        PickImageCommand = new Command(async () => await ExecutePickImageAsync());
        UseCurrentLocationCommand = new Command(async () => await ExecuteUseCurrentLocationAsync());

        // Зареди съществуващия профил ако има
        Task.Run(LoadExistingProfile);
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            _phoneNumber = value;
            OnPropertyChanged();
        }
    }

    public string Address
    {
        get => _address;
        set
        {
            _address = value;
            OnPropertyChanged();
        }
    }

    public string ProfileImage
    {
        get => _profileImage;
        set
        {
            _profileImage = value;
            OnPropertyChanged();
        }
    }
    public ICommand GoBackCommand { get; }
    public ICommand SaveProfileCommand { get; }
    public ICommand PickImageCommand { get; }

    public ICommand UseCurrentLocationCommand { get; }

    private async Task LoadExistingProfile()
    {
        try
        {
            IsBusy = true;

            var profile = await _userProfileService.GetProfileAsync();
            if (profile != null)
            {
                Name = profile.Name;
                PhoneNumber = profile.PhoneNumber;
                Address = profile.Address;
                ProfileImage = profile.Img;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при зареждане на профил: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExecuteUseCurrentLocationAsync()
    {
        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Опит за взимане на текуща локация...");

            if (_locationService == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ _locationService е null!");
                ErrorMessage = "Грешка: LocationService не е инициализиран";
                return;
            }

            var address = await _locationService.GetCurrentAddressAsync();

            if (!string.IsNullOrEmpty(address))
            {
                Address = address;
                System.Diagnostics.Debug.WriteLine($"✅ Адресът е попълнен: {address}");
            }
            else
            {
                ErrorMessage = "Не можа да се вземе текущата локация. Проверете дали GPS-ът е включен.";
            }
        }, "Грешка при взимане на локация");
    }

    private async Task ExecutePickImageAsync()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Изберете снимка"
            });

            if (result != null)
            {
                // Запазваме пътя до снимката
                ProfileImage = result.FullPath;
                // Тук може да качиш снимката към сървъра
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при избор на снимка: {ex.Message}";
        }
    }

    private async Task ExecuteSaveProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Моля въведете име"; return; }
        if (string.IsNullOrWhiteSpace(PhoneNumber)) { ErrorMessage = "Моля въведете телефонен номер"; return; }
        if (string.IsNullOrWhiteSpace(Address)) { ErrorMessage = "Моля въведете адрес"; return; }

        var profile = new UpdateUserProfileDto
        {
            Name        = Name,
            PhoneNumber = PhoneNumber,
            Address     = Address,
            // Img вече го няма тук
        };

        // Вземи байтовете ако има избрана снимка
        byte[]? imageBytes = null;
        string? imageFileName = null;

        if (!string.IsNullOrEmpty(ProfileImage) && File.Exists(ProfileImage))
        {
            imageBytes    = await File.ReadAllBytesAsync(ProfileImage);
            imageFileName = Path.GetFileName(ProfileImage);
        }

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Запазване на профил...");
            var response = await _userProfileService.UpdateProfileAsync(profile, imageBytes, imageFileName);
            if (response != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Профилът запазен. Next: {response.Next}");
                if (!string.IsNullOrEmpty(response.Next))
                    await _navigationService.GoToAsync(response.Next);
                else
                    await _navigationService.GoToAsync("user/home");
            }
        }, "Грешка при запазване на профила");
    }
}