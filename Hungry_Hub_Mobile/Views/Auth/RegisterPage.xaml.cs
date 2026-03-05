using Hungry_Hub_Mobile.ViewModels.Auth;

namespace Hungry_Hub_Mobile.Views.Auth;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        try
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? √решка в RegisterPage constructor: {ex}");
        }
    }
}