using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data.Context;
using RestaurantApp.Data.Repositories.Interfaces;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Repositories.Implementations;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetAllWithItemsAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                    .ThenInclude(m => m.Category)
            .OrderByDescending(o => o.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                    .ThenInclude(m => m.Category)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date.AddDays(1).AddTicks(-1);

        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Where(o => o.Date >= start && o.Date <= end)
            .OrderByDescending(o => o.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = date.Date.AddDays(1).AddTicks(-1);

        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Where(o => o.Date >= start && o.Date <= end)
            .OrderByDescending(o => o.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Where(o => o.TotalAmount >= minPrice && o.TotalAmount <= maxPrice)
            .OrderByDescending(o => o.Date)
            .AsNoTracking()
            .ToListAsync();
    }
}
