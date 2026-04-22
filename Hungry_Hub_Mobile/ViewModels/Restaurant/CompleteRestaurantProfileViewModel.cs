using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.Restaurant;

public class CompleteRestaurantProfileViewModel : BaseViewModel
{
    private readonly IRestaurantProfileService _restaurantProfileService;
    private readonly INavigationService _navigationService;

    private string _name;
    private string _phoneNumber;
    private string _address;
    private string _profileImage;

    public CompleteRestaurantProfileViewModel(
        IRestaurantProfileService restaurantProfileService,
        IAuthService authService,
        INavigationService navigationService) : base(authService, navigationService)
    {
        _restaurantProfileService = restaurantProfileService;
        _navigationService = navigationService;

        SaveProfileCommand = new Command(async () => await ExecuteSaveProfileAsync());
        PickImageCommand = new Command(async () => await ExecutePickImageAsync());

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

    public ICommand SaveProfileCommand { get; }
    public ICommand PickImageCommand { get; }

    private async Task LoadExistingProfile()
    {
        try
        {
            IsBusy = true;

            var response = await _restaurantProfileService.GetProfileAsync();

            if (response?.ProfileExists == true && response.Restaurant != null)
            {
                Name = response.Restaurant.Name;
                PhoneNumber = response.Restaurant.PhoneNumber;
                Address = response.Restaurant.Address;
                ProfileImage = response.Restaurant.Img;
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

    private async Task ExecutePickImageAsync()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Изберете снимка на ресторанта"
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
        // Валидация
        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Моля въведете име на ресторанта";
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

        var profile = new UpdateRestaurantProfileDto
        {
            Name = Name,
            PhoneNumber = PhoneNumber,
            Address = Address,
            Img = ProfileImage  // За сега само пътя
        };

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Запазване на restaurant профил...");

            var response = await _restaurantProfileService.UpdateProfileAsync(profile);

            if (response != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Профилът запазен. Next: {response.Next}");

                // Пренасочване според next параметъра
                if (!string.IsNullOrEmpty(response.Next))
                {
                    await _navigationService.GoToAsync(response.Next);
                }
                else
                {
                    await _navigationService.GoToAsync("restaurant/home");
                }
            }
        }, "Грешка при запазване на профила");
    }
}