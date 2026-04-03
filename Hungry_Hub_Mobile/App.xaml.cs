using Hungry_Hub_Mobile.Views;

namespace Hungry_Hub_Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    // Конструкторът получава IServiceProvider от DI
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        // Стартираме с StartPage, но го създаваме през DI
        //MainPage = new NavigationPage(_serviceProvider.GetService<StartPage>());
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        //return new Window(_serviceProvider.GetService<AppShell>());
        // Вземи StartPage от DI контейнера
        var startPage = _serviceProvider.GetService<StartPage>();

        // Създай нов прозорец с навигация
        return new Window(new NavigationPage(startPage));
    }
}