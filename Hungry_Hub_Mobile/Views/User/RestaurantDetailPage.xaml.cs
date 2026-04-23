using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Views.User;

public partial class RestaurantDetailPage : ContentPage
{
    public RestaurantDetailPage(RestaurantDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}