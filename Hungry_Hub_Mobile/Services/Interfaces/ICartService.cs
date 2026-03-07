using Hungry_Hub_Mobile.Core.DTOs.Orders;

namespace Hungry_Hub_Mobile.Services.Interfaces;

public interface ICartService
{
    /// <summary>
    /// Добавя артикул в количката
    /// </summary>
    Task<AddToCartResponseDto> AddToCartAsync(int articleId);

    /// <summary>
    /// Взема текущата количка
    /// </summary>
    Task<CartResponseDto> GetCartAsync();  // ← ПРОМЕНИ ОТ CartDto НА CartResponseDto

    /// <summary>
    /// Премахва артикул от количката
    /// </summary>
    Task<CartResponseDto> RemoveFromCartAsync(int articleId);
}