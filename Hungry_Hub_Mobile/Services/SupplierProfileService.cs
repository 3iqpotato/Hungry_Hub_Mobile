using System.Text;
using System.Text.Json;
using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Suppliers;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class SupplierProfileService : BaseApiService, ISupplierProfileService
{
    public SupplierProfileService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<SupplierProfileResponseDto> GetProfileAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Вземане на supplier profile");

            return await GetAsync<SupplierProfileResponseDto>(ApiRoutes.Suppliers.CompleteProfile);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при вземане на профил: {ex}");
            throw;
        }
    }

    public async Task<CompleteSupplierResponseDto> UpdateProfileAsync(UpdateSupplierProfileDto profile)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Обновяване на supplier profile");
            System.Diagnostics.Debug.WriteLine($"Name: {profile.Name}");
            System.Diagnostics.Debug.WriteLine($"Phone: {profile.PhoneNumber}");
            System.Diagnostics.Debug.WriteLine($"Type: {profile.Type}");

            var response = await PostAsync<UpdateSupplierProfileDto, CompleteSupplierResponseDto>(
                ApiRoutes.Suppliers.CompleteProfile,
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