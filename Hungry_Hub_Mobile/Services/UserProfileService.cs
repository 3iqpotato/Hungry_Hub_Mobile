using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Users;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;
using System.Net.Http.Headers;
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

    public async Task<CompleteProfileResponseDto> UpdateProfileAsync(
    UpdateUserProfileDto profile,
    byte[]? imageBytes = null,
    string? imageFileName = null)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Обновяване на user profile");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(profile.Name), "name");
            content.Add(new StringContent(profile.PhoneNumber), "phone_number");
            content.Add(new StringContent(profile.Address), "address");

            if (imageBytes != null && imageBytes.Length > 0)
            {
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType =
                    new MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "img", imageFileName ?? "profile.jpg");
            }

            var response = await _httpClient.PostAsync(ApiRoutes.Users.CompleteProfile, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CompleteProfileResponseDto>(json);

            System.Diagnostics.Debug.WriteLine($"✅ Профилът обновен. Next: {result?.Next}");

            if (result?.ProfileId.HasValue == true)
            {
                await TokenStorage.SaveProfileIdAsync(result.ProfileId.Value);
                System.Diagnostics.Debug.WriteLine($"✅ Запазен profile_id: {result.ProfileId.Value}");
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при обновяване на профил: {ex}");
            throw;
        }
    }

    public async Task<UserProfileDto> EditProfileAsync(
        UpdateUserProfileDto profile,
        byte[]? imageBytes = null,
        string? imageFileName = null)
    {
        var profileId = await TokenStorage.GetProfileIdAsync();
        if (!profileId.HasValue)
            throw new Exception("Няма profile_id в storage");

        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(profile.Name), "name");
        content.Add(new StringContent(profile.PhoneNumber), "phone_number");
        content.Add(new StringContent(profile.Address), "address");

        if (imageBytes != null && imageBytes.Length > 0)
        {
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType =
                new MediaTypeHeaderValue("image/jpeg");
            content.Add(imageContent, "img", imageFileName ?? "profile.jpg");
        }

        var url = ApiRoutes.Users.EditUserProfile(profileId.Value);

        // ← само това се промени, всичко друго е същото
        var response = await _httpClient.PutAsync(url, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<EditProfileResponseDto>(json);
        return result?.Profile ?? throw new Exception("Празен отговор от сървъра");
    }

    // Добави и този response DTO във файл
}

