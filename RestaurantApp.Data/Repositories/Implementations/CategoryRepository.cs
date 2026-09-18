using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data.Context;
using RestaurantApp.Data.Repositories.Interfaces;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Repositories.Implementations;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetAllWithMenuItemsAsync()
    {
        return await _context.Categories
            .Include(c => c.MenuItems)
            .AsNoTracking()
            .ToListAsync();
    }
}
