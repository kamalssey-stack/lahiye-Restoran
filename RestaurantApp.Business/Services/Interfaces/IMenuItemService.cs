using RestaurantApp.Business.DTOs.MenuItem;

namespace RestaurantApp.Business.Services.Interfaces;

public interface IMenuItemService
{
    Task<IEnumerable<MenuItemDto>> GetAllAsync();
    Task<MenuItemDto?> GetByIdAsync(int id);
    Task<MenuItemDto> AddAsync(MenuItemCreateDto dto);
    Task EditAsync(int id, MenuItemUpdateDto dto);
    Task RemoveAsync(int id);
    Task<IEnumerable<MenuItemDto>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<MenuItemDto>> GetByPriceIntervalAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<MenuItemDto>> SearchByNameAsync(string searchText);
}
