using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class RestaurantMenuService : BaseApiService, IRestaurantMenuService
{
    public RestaurantMenuService() : base()
    {
    }

    public async Task<MenuForUsersDto> GetMenuForUsersAsync(int restaurantId, string? foodType = null)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Вземане на меню за ресторант ID: {restaurantId}, филтър: {foodType ?? "няма"}");

            // Използваме правилния endpoint от ApiRoutes
            var url = ApiRoutes.Restaurants.MenuForUsers(restaurantId);

            // Добавяме филтър ако има
            if (!string.IsNullOrEmpty(foodType))
            {
                url += $"?type={foodType}";
                System.Diagnostics.Debug.WriteLine($"👉 С филтър: {url}");
            }

            return await GetAsync<MenuForUsersDto>(url);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при вземане на меню: {ex}");
            throw;
        }
    }
}