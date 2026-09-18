using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Repositories.Interfaces;

public interface IMenuItemRepository : IGenericRepository<MenuItem>
{
    Task<IEnumerable<MenuItem>> GetAllWithCategoryAsync();
    Task<MenuItem?> GetByIdWithCategoryAsync(int id);
    Task<MenuItem?> GetByNameAsync(string name);
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task<IEnumerable<MenuItem>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<MenuItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<MenuItem>> SearchByNameAsync(string name);
}
