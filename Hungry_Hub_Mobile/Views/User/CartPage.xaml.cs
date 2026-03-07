using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Views.User;

public partial class CartPage : ContentPage
{
    public CartPage(CartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}