using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            if (response?.ProfileId.HasValue == true)
            {
                await TokenStorage.SaveProfileIdAsync(response.ProfileId.Value);
                System.Diagnostics.Debug.WriteLine($"✅ Запазен profile_id от update: {response.ProfileId.Value}");
            }

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при обновяване на профил: {ex}");
            throw;
        }
    }

    public async Task<UserProfileDto> EditProfileAsync(UpdateUserProfileDto profile)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Редакция на user profile");
            System.Diagnostics.Debug.WriteLine($"Name: {profile.Name}");
            System.Diagnostics.Debug.WriteLine($"Phone: {profile.PhoneNumber}");
            System.Diagnostics.Debug.WriteLine($"Address: {profile.Address}");

            // Вземи profile_id, за да формираш пълния URL
            var profileId = await TokenStorage.GetProfileIdAsync();
            if (!profileId.HasValue)
            {
                throw new Exception("Няма profile_id в storage");
            }

            // Използваме PUT заявка към edit endpoint-а
            var response = await PutAsync<UpdateUserProfileDto, EditProfileResponseDto>(
                ApiRoutes.Users.EditUserProfile(profileId.Value),
                profile);

            System.Diagnostics.Debug.WriteLine($"✅ Профилът редактиран");

            return response?.Profile;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при редакция на профил: {ex}");
            throw;
        }
    }

    // Добави и този response DTO във файла
    public class EditProfileResponseDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("profile")]
        public UserProfileDto Profile { get; set; } = new();
    }
}

