using Hungry_Hub_Mobile.ViewModels.Orders;

namespace Hungry_Hub_Mobile.Views.Orders;

public partial class CheckoutPage : ContentPage
{
    public CheckoutPage(CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}