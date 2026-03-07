using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Views.User;

public partial class RestaurantDetailPage : ContentPage
{
    private readonly RestaurantDetailViewModel _viewModel;

    public RestaurantDetailPage(RestaurantDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Получаваме параметрите при навигация
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is RestaurantDetailViewModel vm)
        {
            // Вземи restaurantId от navigation параметрите
            if (Shell.Current?.CurrentPage?.BindingContext is RestaurantDetailViewModel)
            {
                // Ако ползваме Shell, параметрите идват автоматично
                if (Shell.Current?.CurrentItem?.CurrentItem is IShellSectionController section)
                {
                    // TODO: Вземи параметрите
                }
            }
        }
    }

    // Алтернативен метод - ако ползваме NavigationPage
    public void Initialize(int restaurantId)
    {
        _viewModel.InitializeAsync(restaurantId);
    }
}