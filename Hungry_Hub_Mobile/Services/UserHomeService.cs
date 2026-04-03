using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class UserHomeService : BaseApiService, IUserHomeService
{
    public UserHomeService() : base()
    {
    }

    public async Task<UserHomeDto> GetUserHomeAsync(int profileId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Вземане на user home за profileId: {profileId}");

            return await GetAsync<UserHomeDto>(ApiRoutes.Users.UserHome(profileId));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при вземане на user home: {ex}");
            throw;
        }
    }
}