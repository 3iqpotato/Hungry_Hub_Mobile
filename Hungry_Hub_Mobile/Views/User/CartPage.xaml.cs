using Hungry_Hub_Mobile.ViewModels.User;
namespace Hungry_Hub_Mobile.Views.User;

//namespace Hungry_Hub_Mobile.Views.Cart;

public partial class CartPage : ContentPage
{
    private readonly CartViewModel _viewModel;

    public CartPage(CartViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // ?? Този метод се вика всеки път, когато страницата се появи
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        System.Diagnostics.Debug.WriteLine("?? CartPage OnAppearing - опресняване...");

        if (_viewModel != null)
        {
            await _viewModel.LoadCartAsync();
        }
    }
}