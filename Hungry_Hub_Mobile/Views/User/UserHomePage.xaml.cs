using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Views.User;

public partial class UserHomePage : ContentPage
{
    public UserHomePage(UserHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}