using RestaurantApp.Business.DTOs.Category;

namespace RestaurantApp.Business.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
}
