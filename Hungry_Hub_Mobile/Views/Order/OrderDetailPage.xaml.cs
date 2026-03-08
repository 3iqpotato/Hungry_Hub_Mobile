using Hungry_Hub_Mobile.ViewModels.Orders;

namespace Hungry_Hub_Mobile.Views.Orders;

public partial class OrderDetailPage : ContentPage  // ? Трябва да е OrderDetailPage, а не MyOrdersPage
{
    private readonly OrderDetailViewModel _viewModel;  // ? Трябва да е OrderDetailViewModel

    public OrderDetailPage(OrderDetailViewModel viewModel)  // ? OrderDetailViewModel
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Тук няма нужда от LoadOrdersCommand, защото ViewModel-а се зарежда през OrderId
    }
}