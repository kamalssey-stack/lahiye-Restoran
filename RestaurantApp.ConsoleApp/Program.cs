using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantApp.ConsoleApp.UI;
using RestaurantApp.Data.Context;

namespace RestaurantApp.ConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Restoran Sifaris Idareetme Sistemi";

        var serviceProvider = ServiceRegistration.ConfigureServices();

        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                ConsoleHelper.PrintInfo("Verilenler bazasi yoxlanilir ve hazirlanir...");
                await dbContext.Database.EnsureCreatedAsync();
                ConsoleHelper.PrintSuccess("Verilenler bazasi ugurla qosuldu.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Verilenler bazasina qosulma xetasi: {ex.Message}");
                Console.WriteLine("Davam etmek isteyirsiniz? (Enter basaraq davam edin)");
                Console.ReadLine();
            }

            if (args.Contains("--demo"))
            {
                var demoRunner = scope.ServiceProvider.GetRequiredService<DemoRunner>();
                await demoRunner.RunDemoAsync();
                return;
            }

            var mainMenu = scope.ServiceProvider.GetRequiredService<MainMenu>();
            await mainMenu.RunAsync();
        }
    }
}
