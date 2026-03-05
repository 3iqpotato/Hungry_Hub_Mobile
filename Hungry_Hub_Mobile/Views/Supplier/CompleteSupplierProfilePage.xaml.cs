using Hungry_Hub_Mobile.ViewModels.Supplier;

namespace Hungry_Hub_Mobile.Views.Supplier;

public partial class CompleteSupplierProfilePage : ContentPage
{
    public CompleteSupplierProfilePage(CompleteSupplierProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}