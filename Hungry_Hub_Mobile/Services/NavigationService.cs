using Hungry_Hub_Mobile.Services.Interfaces;
using Hungry_Hub_Mobile.ViewModels.User;

namespace Hungry_Hub_Mobile.Services;

public class NavigationService : INavigationService
{
    // Взимаме текущата навигация от Application.Current.MainPage
    private INavigation GetNavigation()
    {
        if (Application.Current?.MainPage == null)
        {
            System.Diagnostics.Debug.WriteLine("❌ MainPage е null!");
            return null;
        }

        // Ако е NavigationPage, вземаме неговата навигация
        if (Application.Current.MainPage is NavigationPage navigationPage)
        {
            return navigationPage.Navigation;
        }

        // Ако е друга страница, опитваме нейната навигация
        return Application.Current.MainPage.Navigation;
    }

    public async Task GoToAsync(string route)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Опит за навигация към: {route}");

            var navigation = GetNavigation();
            if (navigation == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Navigation е null!");
                return;
            }

            // Търсим страницата по име
            var pageType = GetPageTypeFromRoute(route);
            if (pageType == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Не може да намери страница за route: {route}");
                return;
            }

            // Създаваме страницата през DI
            var page = MauiProgram.Services.GetService(pageType) as Page;
            if (page == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Не може да създаде страница от тип: {pageType.Name}");
                return;
            }

            await navigation.PushAsync(page);
            System.Diagnostics.Debug.WriteLine($"✅ Успешна навигация към {route}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при навигация: {ex.Message}");
        }
    }

    public async Task GoToAsync(string route, Dictionary<string, object> parameters)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Опит за навигация към: {route} с параметри");

            var navigation = GetNavigation();
            if (navigation == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Navigation е null!");
                return;
            }

            var pageType = GetPageTypeFromRoute(route);
            if (pageType == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Не може да намери страница за route: {route}");
                return;
            }

            var page = MauiProgram.Services.GetService(pageType) as Page;
            if (page == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Не може да създаде страница от тип: {pageType.Name}");
                return;
            }

            // Ако страницата поддържа инициализация с параметри
            if (page.BindingContext is RestaurantDetailViewModel vm && parameters.ContainsKey("restaurantId"))
            {
                var restaurantId = (int)parameters["restaurantId"];
                await vm.InitializeAsync(restaurantId);
            }

            await navigation.PushAsync(page);
            System.Diagnostics.Debug.WriteLine($"✅ Успешна навигация към {route}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при навигация: {ex.Message}");
        }
    }

    public async Task GoBackAsync()
    {
        try
        {
            var navigation = GetNavigation();
            if (navigation?.NavigationStack?.Count > 1)
            {
                await navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при връщане: {ex.Message}");
        }
    }

    private Type GetPageTypeFromRoute(string route)
    {
        return route.ToLower() switch
        {
            "login" => typeof(Views.Auth.LoginPage),
            "register" => typeof(Views.Auth.RegisterPage),
            "user_home" => typeof(Views.User.UserHomePage),
            "user/home" => typeof(Views.User.UserHomePage),
            "user/profile" => typeof(Views.User.ProfilePage),
            "user/edit-profile" => typeof(Views.User.EditProfilePage),
            "cart" => typeof(Views.User.CartPage),
            "checkout" => typeof(Views.Orders.CheckoutPage),
            "restaurant/details" => typeof(Views.User.RestaurantDetailPage),
            //"supplier/home" => typeof(Views.Supplier.SupplierHomePage),
            //"restaurant/home" => typeof(Views.Restaurant.RestaurantHomePage),
            "complete_user_profile" => typeof(Views.User.CompleteUserProfilePage),
            "complete_supplier_profile" => typeof(Views.Supplier.CompleteSupplierProfilePage),
            "complete_restaurant_profile" => typeof(Views.Restaurant.CompleteRestaurantProfilePage),
            //"cart" => typeof(Views.User.CartPage),           // ← за после
            //"my-orders" => typeof(Views.User.OrdersPage),    // ← за после
            //"user/profile" => typeof(Views.User.ProfilePage), // ← за после
            _ => null
        };
    }
}