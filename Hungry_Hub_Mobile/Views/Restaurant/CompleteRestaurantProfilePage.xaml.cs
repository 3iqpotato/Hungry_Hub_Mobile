using Hungry_Hub_Mobile.ViewModels.Restaurant;

namespace Hungry_Hub_Mobile.Views.Restaurant;

public partial class CompleteRestaurantProfilePage : ContentPage
{
    public CompleteRestaurantProfilePage(CompleteRestaurantProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}