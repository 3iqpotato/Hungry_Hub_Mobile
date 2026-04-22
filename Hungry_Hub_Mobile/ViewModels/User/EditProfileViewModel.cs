using System.Windows.Input;
using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.User;

public class EditProfileViewModel : BaseViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly INavigationService _navigationService;
    private readonly ILocationService _locationService;

    private string _name;
    private string _phoneNumber;
    private string _address;
    private string _profileImage;
    private int _profileId;

    public EditProfileViewModel(
        IUserProfileService userProfileService,
        ILocationService locationService,
        INavigationService navigationService)

    {
        _userProfileService = userProfileService;
        _locationService = locationService;
        _navigationService = navigationService;

        SaveCommand = new Command(async () => await ExecuteSaveAsync());
        CancelCommand = new Command(async () => await _navigationService.GoBackAsync());
        PickImageCommand = new Command(async () => await ExecutePickImageAsync());
        UseCurrentLocationCommand = new Command(async () => await ExecuteUseCurrentLocationAsync());

        // Зареди съществуващия профил
        //Task.Run(LoadProfileAsync);
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

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand PickImageCommand { get; }

    public ICommand UseCurrentLocationCommand { get; }

    public async Task LoadProfileAsync()
    {
        try
        {
            IsBusy = true;

            var profileId = await _userProfileService.GetCurrentProfileIdAsync();  //TODO
            if (!profileId.HasValue)
            {
                ErrorMessage = "Не е намерен profile_id";
                return;
            }

            _profileId = profileId.Value;

            var profile = await _userProfileService.GetProfileAsync();
            if (profile != null)
            {
                Name = profile.Name;
                PhoneNumber = profile.PhoneNumber;
                Address = profile.Address;
                ProfileImage = profile.Img;

                System.Diagnostics.Debug.WriteLine($"✅ Профил зареден за редакция");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при зареждане: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExecutePickImageAsync()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Изберете снимка"
            });

            if (result == null)
                return;

            // Проверка на размера ПРЕДИ да запазваме пътя
            var fileInfo = new FileInfo(result.FullPath);
            if (fileInfo.Length > AppConstants.MaxImageSizeBytes)
            {
                ErrorMessage = $"Снимката е твърде голяма. Максималният размер е {AppConstants.MaxImageSizeLabel}.";
                return;
            }

            ProfileImage = result.FullPath;
            ErrorMessage = null; // изчисти стара грешка ако има
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при избор на снимка: {ex.Message}";
        }
    }

    private async Task ExecuteUseCurrentLocationAsync()
    {
        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Опит за взимане на текуща локация...");

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

    private async Task ExecuteSaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Моля въведете име"; return; }
        if (string.IsNullOrWhiteSpace(PhoneNumber)) { ErrorMessage = "Моля въведете телефонен номер"; return; }
        if (string.IsNullOrWhiteSpace(Address)) { ErrorMessage = "Моля въведете адрес"; return; }

        var profileDto = new UpdateUserProfileDto
        {
            Name        = Name,
            PhoneNumber = PhoneNumber,
            Address     = Address,
            // Img вече го няма тук
        };

        // Вземи байтовете ако има избрана нова снимка
        byte[]? imageBytes = null;
        string? imageFileName = null;

        if (!string.IsNullOrEmpty(ProfileImage)
            && !ProfileImage.StartsWith("http")) // не е стар URL — нова снимка
        {
            if (File.Exists(ProfileImage))
            {
                imageBytes    = await File.ReadAllBytesAsync(ProfileImage);
                imageFileName = Path.GetFileName(ProfileImage);
            }
        }

        await ExecuteAsync(async () =>
        {
            var updated = await _userProfileService.EditProfileAsync(
                profileDto, imageBytes, imageFileName);

            if (updated != null)
                await _navigationService.GoBackAsync();

        }, "Грешка при запазване");
    }
}