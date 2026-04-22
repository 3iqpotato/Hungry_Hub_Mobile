using Hungry_Hub_Mobile.Core.DTOs.Restaurants;
using Hungry_Hub_Mobile.Core.DTOs.Users;

namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface IUserHomeService
{
    /// <summary>
    /// Взема началните данни за потребител
    /// </summary>
    Task<UserHomeDto> GetUserHomeAsync(int profileId);

    Task<UserHomeDto> GetUserHomeAsync();

}