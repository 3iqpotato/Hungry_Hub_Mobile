using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Views.User;

public partial class CompleteUserProfilePage : ContentPage
{
    public CompleteUserProfilePage(CompleteUserProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}