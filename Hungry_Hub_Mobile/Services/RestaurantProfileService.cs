using System.Text;
using System.Text.Json;
using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class RestaurantProfileService : BaseApiService, IRestaurantProfileService
{
    public RestaurantProfileService() : base()
    {
    }

    public async Task<RestaurantProfileResponseDto> GetProfileAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Вземане на restaurant profile");

            return await GetAsync<RestaurantProfileResponseDto>(ApiRoutes.Restaurants.CompleteProfile);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при вземане на профил: {ex}");
            throw;
        }
    }

    public async Task<CompleteRestaurantResponseDto> UpdateProfileAsync(UpdateRestaurantProfileDto profile)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Обновяване на restaurant profile");
            System.Diagnostics.Debug.WriteLine($"Name: {profile.Name}");
            System.Diagnostics.Debug.WriteLine($"Phone: {profile.PhoneNumber}");
            System.Diagnostics.Debug.WriteLine($"Address: {profile.Address}");

            // Използваме POST към същия endpoint
            var response = await PostAsync<UpdateRestaurantProfileDto, CompleteRestaurantResponseDto>(
                ApiRoutes.Restaurants.CompleteProfile,
                profile);

            System.Diagnostics.Debug.WriteLine($"✅ Профилът обновен. Next: {response?.Next}");
            System.Diagnostics.Debug.WriteLine($"Restaurant ID: {response?.RestaurantId}");

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при обновяване на профил: {ex}");
            throw;
        }
    }
}