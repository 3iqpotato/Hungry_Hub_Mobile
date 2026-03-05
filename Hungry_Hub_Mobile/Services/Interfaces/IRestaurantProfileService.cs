using Hungry_Hub_Mobile.Core.DTOs.Restaurants;

namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface IRestaurantProfileService
{
    /// <summary>
    /// Взема текущия профил на ресторанта
    /// </summary>
    Task<RestaurantProfileResponseDto> GetProfileAsync();

    /// <summary>
    /// Създава или обновява профила на ресторанта
    /// </summary>
    Task<CompleteRestaurantResponseDto> UpdateProfileAsync(UpdateRestaurantProfileDto profile);
}