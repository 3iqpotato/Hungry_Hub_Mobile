using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.User;

public class EditProfileViewModel : BaseViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly INavigationService _navigationService;

    private string _name;
    private string _phoneNumber;
    private string _address;
    private string _profileImage;
    private int _profileId;

    public EditProfileViewModel(
        IUserProfileService userProfileService,
        INavigationService navigationService)
    {
        _userProfileService = userProfileService;
        _navigationService = navigationService;

        SaveCommand = new Command(async () => await ExecuteSaveAsync());
        CancelCommand = new Command(async () => await _navigationService.GoBackAsync());
        PickImageCommand = new Command(async () => await ExecutePickImageAsync());

        // Зареди съществуващия профил
        Task.Run(LoadProfileAsync);
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

    private async Task LoadProfileAsync()
    {
        try
        {
            IsBusy = true;

            var profileId = await TokenStorage.GetProfileIdAsync();
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

            if (result != null)
            {
                ProfileImage = result.FullPath;
                // Забележка: За да качиш снимка, трябва да конвертираш до base64 или byte array
                // Това ще го добавим после
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при избор на снимка: {ex.Message}";
        }
    }

    private async Task ExecuteSaveAsync()
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Моля въведете име";
            return;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ErrorMessage = "Моля въведете телефонен номер";
            return;
        }

        if (string.IsNullOrWhiteSpace(Address))
        {
            ErrorMessage = "Моля въведете адрес";
            return;
        }

        var profile = new UpdateUserProfileDto
        {
            Name = Name,
            PhoneNumber = PhoneNumber,
            Address = Address,
            Img = ProfileImage
        };

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Запазване на промените...");

            var updatedProfile = await _userProfileService.EditProfileAsync(profile);

            if (updatedProfile != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Профилът обновен");

                // Върни се към профила
                await _navigationService.GoBackAsync();
            }
        }, "Грешка при запазване");
    }
}