using System.Windows.Input;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels.User;

public class ProfileViewModel : BaseViewModel
{
    private readonly IUserProfileService _userProfileService;
    //private readonly INavigationService _navigationService;

    private UserProfileDto _profile = new();

    public ProfileViewModel(
        IUserProfileService userProfileService,
        IAuthService authService,
        INavigationService navigationService) : base(authService, navigationService)
    {
        _userProfileService = userProfileService;
        //_navigationService = navigationService;

        GoToHomeCommand = new Command(async () => await _navigationService.GoToAsync("user_home"));
        GoToEditCommand = new Command(async () => await _navigationService.GoToAsync("user/edit-profile"));
        GoToCartCommand = new Command(async () => await _navigationService.GoToAsync("cart"));
        GoToOrdersCommand = new Command(async () => await _navigationService.GoToAsync("my-orders"));
        //Task.Run(LoadProfileAsync);

    }


    public UserProfileDto Profile
    {
        get => _profile;
        set
        {
            _profile = value;
            OnPropertyChanged();
        }
    }

    public ICommand GoToHomeCommand { get; }
    public ICommand GoToEditCommand { get; }
    public ICommand GoToCartCommand { get; }
    public ICommand GoToOrdersCommand { get; }
    public async Task LoadProfileAsync()
    {
        try
        {
            IsBusy = true;

            var profileId = await _userProfileService.GetCurrentProfileIdAsync();
            if (profileId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"👉 Зареждане на профил за profileId: {profileId.Value}");

                var profile = await _userProfileService.GetProfileAsync();
                if (profile != null)
                {
                    Profile = profile;
                    System.Diagnostics.Debug.WriteLine($"✅ Профил зареден: {profile.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Грешка при зареждане на профил: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }


}

