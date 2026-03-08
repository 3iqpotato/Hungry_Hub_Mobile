using Hungry_Hub_Mobile.Core.Constants;
using Hungry_Hub_Mobile.Core.DTOs.Orders;
using Hungry_Hub_Mobile.Core.Helpers;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class OrderService : BaseApiService, IOrderService
{
    public OrderService() : base()
    {
    }

    public async Task<CreateOrderResponseDto> CreateOrderAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Създаване на поръчка...");

            var response = await PostAsync<object, CreateOrderResponseDto>(
                ApiRoutes.Orders.CreateOrder,
                new { });  // Празно body, POST заявка

            System.Diagnostics.Debug.WriteLine($"✅ Поръчката създадена. ID: {response?.OrderId}, Next: {response?.Next}");

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при създаване на поръчка: {ex.Message}");
            throw;
        }
    }

    public async Task<OrderDto> GetOrderAsync(int orderId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"👉 Вземане на поръчка {orderId}");

            var order = await GetAsync<OrderDto>(ApiRoutes.Orders.OrderDetail(orderId));

            System.Diagnostics.Debug.WriteLine($"✅ Поръчката взета. Статус: {order?.Status}");

            return order;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex.Message}");
            throw;
        }
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Вземане на моите поръчки");

            var orders = await GetAsync<List<OrderDto>>(ApiRoutes.Users.MyOrders);

            System.Diagnostics.Debug.WriteLine($"✅ Заредени {orders?.Count ?? 0} поръчки");

            return orders ?? new List<OrderDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка: {ex.Message}");
            throw;
        }
    }
}