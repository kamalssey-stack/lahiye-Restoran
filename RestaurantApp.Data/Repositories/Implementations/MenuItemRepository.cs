using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data.Context;
using RestaurantApp.Data.Repositories.Interfaces;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Repositories.Implementations;

public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MenuItem>> GetAllWithCategoryAsync()
    {
        return await _context.MenuItems
            .Include(m => m.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<MenuItem?> GetByIdWithCategoryAsync(int id)
    {
        return await _context.MenuItems
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MenuItem?> GetByNameAsync(string name)
    {
        return await _context.MenuItems
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Name.ToLower() == name.Trim().ToLower());
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var query = _context.MenuItems.AsQueryable();
        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }
        return await query.AnyAsync(m => m.Name.ToLower() == name.Trim().ToLower());
    }

    public async Task<IEnumerable<MenuItem>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.MenuItems
            .Include(m => m.Category)
            .Where(m => m.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<MenuItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.MenuItems
            .Include(m => m.Category)
            .Where(m => m.Price >= minPrice && m.Price <= maxPrice)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<MenuItem>> SearchByNameAsync(string name)
    {
        var searchLower = name.Trim().ToLower();
        return await _context.MenuItems
            .Include(m => m.Category)
            .Where(m => m.Name.ToLower().Contains(searchLower))
            .AsNoTracking()
            .ToListAsync();
    }
}
