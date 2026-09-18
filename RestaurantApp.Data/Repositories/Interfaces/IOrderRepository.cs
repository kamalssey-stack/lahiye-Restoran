using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetAllWithItemsAsync();
    Task<Order?> GetByIdWithItemsAsync(int id);
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Order>> GetByDateAsync(DateTime date);
    Task<IEnumerable<Order>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
}
