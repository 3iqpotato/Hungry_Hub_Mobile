using Hungry_Hub_Mobile.Core.DTOs.Orders;

namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Създава нова поръчка от текущата количка
    /// </summary>
    Task<CreateOrderResponseDto> CreateOrderAsync();

    /// <summary>
    /// Взема поръчка по ID
    /// </summary>
    Task<OrderDto> GetOrderAsync(int orderId);

    /// <summary>
    /// Взема всички поръчки на потребителя
    /// </summary>
    Task<List<OrderDto>> GetMyOrdersAsync();

    Task<OrderDetailResponseDto> GetOrderDetailAsync(int orderId);
}