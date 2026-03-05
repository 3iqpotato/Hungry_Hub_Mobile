using System.Text;
using System.Text.Json;
using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class UserProfileService : BaseApiService, IUserProfileService
{
    public UserProfileService() : base()
    {
    }

    public async Task<UserProfileDto> GetProfileAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Вземане на user profile");

            return await GetAsync<UserProfileDto>(ApiRoutes.Users.CompleteProfile);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при вземане на профил: {ex}");
            throw;
        }
    }

    public async Task<CompleteProfileResponseDto> UpdateProfileAsync(UpdateUserProfileDto profile)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Обновяване на user profile");
            System.Diagnostics.Debug.WriteLine($"Name: {profile.Name}");
            System.Diagnostics.Debug.WriteLine($"Phone: {profile.PhoneNumber}");
            System.Diagnostics.Debug.WriteLine($"Address: {profile.Address}");

            // Използваме POST към същия endpoint
            var response = await PostAsync<UpdateUserProfileDto, CompleteProfileResponseDto>(
                ApiRoutes.Users.CompleteProfile,
                profile);

            System.Diagnostics.Debug.WriteLine($"✅ Профилът обновен. Next: {response?.Next}");

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при обновяване на профил: {ex}");
            throw;
        }
    }
}