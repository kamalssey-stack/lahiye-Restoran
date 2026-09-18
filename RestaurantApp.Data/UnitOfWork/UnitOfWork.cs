using RestaurantApp.Data.Context;
using RestaurantApp.Data.Repositories.Implementations;
using RestaurantApp.Data.Repositories.Interfaces;

namespace RestaurantApp.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IMenuItemRepository? _menuItems;
    private IOrderRepository? _orders;
    private ICategoryRepository? _categories;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IMenuItemRepository MenuItems => _menuItems ??= new MenuItemRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
