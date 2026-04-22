using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Views.User;

public partial class CompleteUserProfilePage : ContentPage
{
    private readonly CompleteUserProfileViewModel _viewModel;

    public CompleteUserProfilePage(CompleteUserProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel != null)
        {
            await _viewModel.LoadExistingProfile();
        }
    }
}