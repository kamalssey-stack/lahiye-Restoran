using RestaurantApp.Data.Repositories.Interfaces;

namespace RestaurantApp.Data.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    IMenuItemRepository MenuItems { get; }
    IOrderRepository Orders { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync();
}
