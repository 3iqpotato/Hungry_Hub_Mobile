using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Views.User;

public partial class UserHomePage : ContentPage
{
    private readonly UserHomeViewModel _viewModel;

    public UserHomePage(UserHomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //load data!!!
        if (_viewModel != null)
        {
            await _viewModel.LoadUserHomeAsync();
        }
    }
}