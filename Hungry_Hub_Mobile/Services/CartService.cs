using System.Text;
using System.Text.Json;
using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class CartService : BaseApiService, ICartService
{
    // HttpClient идва от DI
    public CartService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<AddToCartResponseDto> AddToCartAsync(int articleId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Добавяне на артикул {articleId} в количката");

            var response = await PostAsync<object, AddToCartResponseDto>(
                ApiRoutes.Orders.AddToCart(articleId),
                new { });

            System.Diagnostics.Debug.WriteLine($"✅ Добавено в количката. Успех: {response?.Success}");
            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при добавяне в количка: {ex}");
            throw;
        }
    }

    public async Task<CartResponseDto> GetCartAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Вземане на количката");

            var profileId = await TokenStorage.GetProfileIdAsync();
            if (!profileId.HasValue)
                throw new Exception("Няма profile_id");

            var response = await GetAsync<CartResponseDto>(ApiRoutes.Users.UserCart(profileId.Value));

            System.Diagnostics.Debug.WriteLine($"✅ Количката заредена. Брой артикули: {response?.Cart?.Items?.Count ?? 0}");
            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при вземане на количка: {ex}");
            throw;
        }
    }

    public async Task<CartDto> RemoveFromCartAsync(int cartItemId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Премахване на артикул: {cartItemId}");

            var response = await PostAsync<object, CartResponseDto>(
                ApiRoutes.Orders.RemoveFromCart(cartItemId),
                new { });

            System.Diagnostics.Debug.WriteLine($"✅ Премахнат артикул. Items в cart: {response?.Cart?.Items?.Count ?? 0}");
            return response.Cart;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при премахване: {ex}");
            throw;
        }
    }
}