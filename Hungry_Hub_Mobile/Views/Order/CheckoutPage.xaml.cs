using Hungry_Hub_Mobile.ViewModels.Orders;

namespace Hungry_Hub_Mobile.Views.Orders;

public partial class CheckoutPage : ContentPage
{
    private readonly CheckoutViewModel _viewModel;

    public CheckoutPage(CheckoutViewModel viewModel)
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
            await _viewModel.LoadCartAsync();
        }
    }
}