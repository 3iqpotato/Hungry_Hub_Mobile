using Hungry_Hub_Mobile.ViewModels.Orders;

namespace Hungry_Hub_Mobile.Views.Orders;

public partial class MyOrdersPage : ContentPage
{
    private readonly MyOrdersViewModel _viewModel;

    public MyOrdersPage(MyOrdersViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadOrdersCommand.Execute(null);
    }
}