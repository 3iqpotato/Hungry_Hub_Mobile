using Hungry_Hub_Mobile.ViewModels;

namespace Hungry_Hub_Mobile.Views;

public partial class StartPage : ContentPage
{
    private readonly StartPageViewModel _viewModel;

    public StartPage(StartPageViewModel viewModel)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("?? StartPage constructor START");
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;

            System.Diagnostics.Debug.WriteLine("? BindingContext set");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Грешка в StartPage constructor: {ex}");
        }
    }

    // ?? Този метод се вика всеки път, когато страницата се появи
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        System.Diagnostics.Debug.WriteLine("?? StartPage OnAppearing - презареждане на статус");

        if (_viewModel != null)
        {
            await _viewModel.RefreshLoginStatusAsync();
        }
    }
}