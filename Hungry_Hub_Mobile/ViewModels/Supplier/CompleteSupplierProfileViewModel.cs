using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Suppliers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.Supplier;

public class CompleteSupplierProfileViewModel : BaseViewModel
{
    private readonly ISupplierProfileService _supplierProfileService;
    private readonly INavigationService _navigationService;

    private string _name;
    private string _phoneNumber;
    private string _profileImage;
    private TransportTypeItem? _selectedTransportType;
    private List<TransportTypeItem> _transportTypes;

    public CompleteSupplierProfileViewModel(
        ISupplierProfileService supplierProfileService,
        IAuthService authService,
        INavigationService navigationService) : base(authService, navigationService)
    {
        _supplierProfileService = supplierProfileService;
        _navigationService = navigationService;

        // Създаваме списъка директно тук
        _transportTypes = new List<TransportTypeItem>
        {
            new TransportTypeItem { Value = "car", DisplayName = "Кола" },
            new TransportTypeItem { Value = "motorcycle", DisplayName = "Мотор" },
            new TransportTypeItem { Value = "bicycle", DisplayName = "Велосипед" },
            new TransportTypeItem { Value = "other", DisplayName = "Друг" }
        };

        SaveProfileCommand = new Command(async () => await ExecuteSaveProfileAsync());
        PickImageCommand = new Command(async () => await ExecutePickImageAsync());

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

    public string ProfileImage
    {
        get => _profileImage;
        set
        {
            _profileImage = value;
            OnPropertyChanged();
        }
    }

    public List<TransportTypeItem> TransportTypes
    {
        get => _transportTypes;
        set
        {
            _transportTypes = value;
            OnPropertyChanged();
        }
    }

    public TransportTypeItem? SelectedTransportType
    {
        get => _selectedTransportType;
        set
        {
            _selectedTransportType = value;
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

            var response = await _supplierProfileService.GetProfileAsync();

            if (response?.ProfileExists == true && response.Supplier != null)
            {
                Name = response.Supplier.Name;
                PhoneNumber = response.Supplier.PhoneNumber;
                ProfileImage = response.Supplier.Img;

                // Намери избрания транспорт
                if (!string.IsNullOrEmpty(response.Supplier.Type))
                {
                    SelectedTransportType = TransportTypes
                        .FirstOrDefault(t => t.Value == response.Supplier.Type);
                }
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
                Title = "Изберете снимка"
            });

            if (result != null)
            {
                ProfileImage = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при избор на снимка: {ex.Message}";
        }
    }

    private async Task ExecuteSaveProfileAsync()
    {
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

        if (SelectedTransportType == null)
        {
            ErrorMessage = "Моля изберете тип превозно средство";
            return;
        }

        var profile = new UpdateSupplierProfileDto
        {
            Name = Name,
            PhoneNumber = PhoneNumber,
            Type = SelectedTransportType.Value,
            Img = ProfileImage
        };

        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Запазване на supplier профил...");

            var response = await _supplierProfileService.UpdateProfileAsync(profile);

            if (response != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Профилът запазен. Next: {response.Next}");

                if (!string.IsNullOrEmpty(response.Next))
                {
                    await _navigationService.GoToAsync(response.Next);
                }
                else
                {
                    await _navigationService.GoToAsync("supplier/home");
                }
            }
        }, "Грешка при запазване на профила");
    }
}