namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface ILocationService
{
    /// <summary>
    /// Взема текущата локация и я превръща в адрес
    /// </summary>
    Task<string> GetCurrentAddressAsync();

    /// <summary>
    /// Взема само координатите
    /// </summary>
    Task<(double Latitude, double Longitude)?> GetCurrentCoordinatesAsync();
}