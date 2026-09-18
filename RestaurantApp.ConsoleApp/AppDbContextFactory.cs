using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RestaurantApp.Data.Context;

namespace RestaurantApp.ConsoleApp;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public const string ConnectionString = "Server=.\\MAIN;Database=RestaurantDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(ConnectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
