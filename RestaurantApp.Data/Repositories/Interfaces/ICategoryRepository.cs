using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithMenuItemsAsync();
}
