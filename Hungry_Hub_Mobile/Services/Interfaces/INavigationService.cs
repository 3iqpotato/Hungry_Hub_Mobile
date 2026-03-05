namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface INavigationService
{
    Task GoToAsync(string route);
    Task GoToAsync(string route, Dictionary<string, object> parameters);
    Task GoBackAsync();
}