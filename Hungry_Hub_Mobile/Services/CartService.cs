using System.Text;
using System.Text.Json;
using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class CartService : BaseApiService, ICartService
{
    public CartService() : base()
    {
    }

    public async Task<AddToCartResponseDto> AddToCartAsync(int articleId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Добавяне на артикул {articleId} в количката");

            // POST към /api/orders/add-to-cart/{articleId}/
            var response = await PostAsync<object, AddToCartResponseDto>(
                ApiRoutes.Orders.AddToCart(articleId),
                new { });  // Празен обект, защото article_id е в URL-то

            System.Diagnostics.Debug.WriteLine($"✅ Добавено в количката. Успех: {response?.Success}");

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при добавяне в количка: {ex}");
            throw;
        }
    }

    public async Task<CartResponseDto> GetCartAsync()  // ← Промени типа на връщане
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Вземане на количката");

            var profileId = await TokenStorage.GetProfileIdAsync();
            if (!profileId.HasValue)
            {
                throw new Exception("Няма profile_id");
            }

            // Това вече връща CartResponseDto
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

            // 🔥 Вместо да ползваме PostAsync, правим всичко ръчно
            var url = ApiRoutes.Orders.RemoveFromCart(cartItemId);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            System.Diagnostics.Debug.WriteLine($"Response Status: {(int)response.StatusCode}");

            var responseJson = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Response JSON: {responseJson}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Грешка: {response.StatusCode}");
            }

            // 🔥 Първо десериализирай до обект, за да видиш структурата
            using JsonDocument doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            // Провери дали има "cart" пропърти
            if (root.TryGetProperty("cart", out var cartElement))
            {
                // Директно десериализирай cart пропъртито до CartDto
                var cart = JsonSerializer.Deserialize<CartDto>(cartElement.GetRawText(), _jsonOptions);

                System.Diagnostics.Debug.WriteLine($"✅ Ръчно десериализиран cart. Items: {cart?.Items?.Count ?? 0}");

                if (cart?.Items != null)
                {
                    foreach (var item in cart.Items)
                    {
                        System.Diagnostics.Debug.WriteLine($"   - {item.ArticleName} (ID: {item.Id})");
                    }
                }

                return cart;
            }
            else
            {
                // Ако няма "cart", опитай да десериализираш целия response
                var cart = JsonSerializer.Deserialize<CartDto>(responseJson, _jsonOptions);
                System.Diagnostics.Debug.WriteLine($"✅ Десериализиран като цял response. Items: {cart?.Items?.Count ?? 0}");
                return cart;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex.Message}");
            throw;
        }
    }

}