using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services;
using Hungry_Hub_Mobile.Services.Interfaces;
using Hungry_Hub_Mobile.ViewModels;
using Hungry_Hub_Mobile.ViewModels.Auth;
using Hungry_Hub_Mobile.ViewModels.Orders;
using Hungry_Hub_Mobile.ViewModels.Restaurant;
using Hungry_Hub_Mobile.ViewModels.Supplier;
using Hungry_Hub_Mobile.ViewModels.User;
using Hungry_Hub_Mobile.Views;
using Hungry_Hub_Mobile.Views.Auth;
using Hungry_Hub_Mobile.Views.Orders;
using Hungry_Hub_Mobile.Views.Restaurant;
using Hungry_Hub_Mobile.Views.Supplier;
using Hungry_Hub_Mobile.Views.User;
using Microsoft.Extensions.DependencyInjection;

namespace Hungry_Hub_Mobile;

public static class MauiProgram
{
    public static IServiceProvider? Services { get; private set; }
    public static MauiApp CreateMauiApp()

    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        builder.Services.AddSingleton<AuthenticatedHttpClientHandler>();

        builder.Services.AddSingleton(sp =>
        {
            var handler = sp.GetRequiredService<AuthenticatedHttpClientHandler>();
            return new HttpClient(handler)
            {
                BaseAddress = new Uri(AppConstants.FullBaseApiUrl)
            };
        });
        // ========== SERVICES ==========
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IUserProfileService, UserProfileService>();
        builder.Services.AddSingleton<IRestaurantProfileService, RestaurantProfileService>();
        builder.Services.AddSingleton<ISupplierProfileService, SupplierProfileService>();
        builder.Services.AddSingleton<IUserHomeService, UserHomeService>();
        builder.Services.AddSingleton<IRestaurantMenuService, RestaurantMenuService>();
        builder.Services.AddSingleton<ICartService, CartService>();
        builder.Services.AddSingleton<IOrderService, OrderService>();
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddSingleton<AppShell>();

        // ========== VIEWMODELS ==========
        builder.Services.AddTransient<StartPageViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<CompleteUserProfileViewModel>();
        builder.Services.AddTransient<CompleteRestaurantProfileViewModel>();
        builder.Services.AddTransient<CompleteSupplierProfileViewModel>();
        builder.Services.AddTransient<UserHomeViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<EditProfileViewModel>();
        builder.Services.AddTransient<RestaurantDetailViewModel>();
        builder.Services.AddTransient<CartViewModel>();
        builder.Services.AddTransient<CheckoutViewModel>();
        builder.Services.AddTransient<MyOrdersViewModel>();
        builder.Services.AddTransient<OrderDetailViewModel>();
        // Добави и други ViewModels като ги създаваме

        // ========== VIEWS (PAGES) ==========
        builder.Services.AddTransient<StartPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<CompleteUserProfilePage>();
        builder.Services.AddTransient<CompleteRestaurantProfilePage>();
        builder.Services.AddTransient<CompleteSupplierProfilePage>();
        builder.Services.AddTransient<UserHomePage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<EditProfilePage>();
        builder.Services.AddTransient<RestaurantDetailPage>();
        builder.Services.AddTransient<CartPage>();
        builder.Services.AddTransient<CheckoutPage>();
        builder.Services.AddTransient<MyOrdersPage>();
        builder.Services.AddTransient<OrderDetailPage>();


        var app = builder.Build();
        Services = app.Services;  // Запази Services за достъп отвсякъде

        return app;
    }
}