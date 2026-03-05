using Hungry_Hub_Mobile.Services;
using Hungry_Hub_Mobile.Services.Interfaces;
using Hungry_Hub_Mobile.ViewModels;
using Hungry_Hub_Mobile.ViewModels.Auth;
using Hungry_Hub_Mobile.ViewModels.Restaurant;
using Hungry_Hub_Mobile.ViewModels.Supplier;
using Hungry_Hub_Mobile.ViewModels.User;
using Hungry_Hub_Mobile.Views;
using Hungry_Hub_Mobile.Views.Auth;
using Hungry_Hub_Mobile.Views.Restaurant;
using Hungry_Hub_Mobile.Views.Supplier;
using Hungry_Hub_Mobile.Views.User;

//using Hungry_Hub_Mobile.Views.User;        // За CompleteUserProfilePage
//using Hungry_Hub_Mobile.Views.Supplier;     // За CompleteSupplierProfilePage
//using Hungry_Hub_Mobile.Views.Restaurant;   // За CompleteRestaurantProfilePage
using Microsoft.Extensions.DependencyInjection;

namespace Hungry_Hub_Mobile;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }
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

        // ========== SERVICES ==========
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IUserProfileService, UserProfileService>();
        builder.Services.AddSingleton<IRestaurantProfileService, RestaurantProfileService>();
        builder.Services.AddSingleton<ISupplierProfileService, SupplierProfileService>();
        builder.Services.AddSingleton<IUserHomeService, UserHomeService>();

        // ========== VIEWMODELS ==========
        builder.Services.AddTransient<StartPageViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<CompleteUserProfileViewModel>();
        builder.Services.AddTransient<CompleteRestaurantProfileViewModel>();
        builder.Services.AddTransient<CompleteSupplierProfileViewModel>();
        builder.Services.AddTransient<UserHomeViewModel>();
        // Добави и други ViewModels като ги създаваме

        // ========== VIEWS (PAGES) ==========
        builder.Services.AddTransient<StartPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<CompleteUserProfilePage>();
        builder.Services.AddTransient<CompleteRestaurantProfilePage>();
        builder.Services.AddTransient<CompleteSupplierProfilePage>();
        builder.Services.AddTransient<UserHomePage>();

        // ⚠️ ВАЖНО: Страници за попълване на профил (ако ги създадем после)
        // builder.Services.AddTransient<CompleteUserProfilePage>();
        // builder.Services.AddTransient<CompleteSupplierProfilePage>();
        // builder.Services.AddTransient<CompleteRestaurantProfilePage>();
        var app = builder.Build();
        Services = app.Services;  // Запази Services за достъп отвсякъде

        return app;
    }
}