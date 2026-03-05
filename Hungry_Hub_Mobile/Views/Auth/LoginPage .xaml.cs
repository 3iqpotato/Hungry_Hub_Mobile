using Hungry_Hub_Mobile.ViewModels.Auth;

namespace Hungry_Hub_Mobile.Views.Auth;

public partial class LoginPage : ContentPage
{
    // Конструкторът получава ViewModel-а от Dependency Injection
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}