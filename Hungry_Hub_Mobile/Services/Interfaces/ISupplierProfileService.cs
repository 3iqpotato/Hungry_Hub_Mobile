using Hungry_Hub_Mobile.Core.DTOs.Suppliers;

namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface ISupplierProfileService
{
    /// <summary>
    /// Взема текущия профил на доставчика
    /// </summary>
    Task<SupplierProfileResponseDto> GetProfileAsync();

    /// <summary>
    /// Създава или обновява профила на доставчика
    /// </summary>
    Task<CompleteSupplierResponseDto> UpdateProfileAsync(UpdateSupplierProfileDto profile);
}