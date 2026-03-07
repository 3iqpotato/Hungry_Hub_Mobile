using Hungry_Hub_Mobile.Core.DTOs.Restaurants;

namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface IRestaurantMenuService
{
    /// <summary>
    /// Взема менюто за ресторант с опционален филтър по тип храна
    /// </summary>
    Task<MenuForUsersDto> GetMenuForUsersAsync(int restaurantId, string? foodType = null);
}