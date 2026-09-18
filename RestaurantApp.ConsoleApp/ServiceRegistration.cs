using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantApp.Business.Caching;
using RestaurantApp.Business.Mapping;
using RestaurantApp.Business.Services.Implementations;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.Data.Context;
using RestaurantApp.Data.Repositories.Implementations;
using RestaurantApp.Data.Repositories.Interfaces;
using RestaurantApp.Data.UnitOfWork;
using RestaurantApp.ConsoleApp.UI;

namespace RestaurantApp.ConsoleApp;

public static class ServiceRegistration
{
    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Database Context
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(AppDbContextFactory.ConnectionString));

        // Logging
        services.AddLogging();

        // Memory Cache
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        // AutoMapper
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        // Repositories & UnitOfWork
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Business Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<IOrderService, OrderService>();

        // UI Components
        services.AddScoped<MenuItemMenu>();
        services.AddScoped<OrderMenu>();
        services.AddScoped<MainMenu>();
        services.AddScoped<DemoRunner>();

        return services.BuildServiceProvider();
    }
}
