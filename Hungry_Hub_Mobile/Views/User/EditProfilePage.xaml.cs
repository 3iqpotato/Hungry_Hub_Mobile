using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Views.User;

public partial class EditProfilePage : ContentPage
{
    private readonly EditProfileViewModel _viewModel;

    public EditProfilePage(EditProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 🔥 Зареждане на профила, когато страницата се появи
        if (_viewModel != null)
        {
            await _viewModel.LoadProfileAsync();
        }
    }
}