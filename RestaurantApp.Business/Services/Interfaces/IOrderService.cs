using RestaurantApp.Business.DTOs.Order;

namespace RestaurantApp.Business.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDetailDto> AddOrderAsync(OrderCreateDto dto);
    Task RemoveOrderAsync(int id);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<IEnumerable<OrderDto>> GetOrdersByDatesIntervalAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<OrderDto>> GetOrdersByDateAsync(DateTime date);
    Task<IEnumerable<OrderDto>> GetOrdersByPriceIntervalAsync(decimal minPrice, decimal maxPrice);
    Task<OrderDetailDto?> GetOrderByNoAsync(int id);
}
