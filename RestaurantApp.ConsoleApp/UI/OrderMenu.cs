using RestaurantApp.Business.DTOs.Order;
using RestaurantApp.Business.Services.Interfaces;

namespace RestaurantApp.ConsoleApp.UI;

public class OrderMenu
{
    private readonly IOrderService _orderService;
    private readonly IMenuItemService _menuItemService;

    public OrderMenu(IOrderService orderService, IMenuItemService menuItemService)
    {
        _orderService = orderService;
        _menuItemService = menuItemService;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader("SIFARISLER UZERINDE EMELIYYATLAR");
            Console.WriteLine("1. Yeni sifaris elave etmek");
            Console.WriteLine("2. Sifarisin legvi");
            Console.WriteLine("3. Butun sifarislerin ekrana cixarilmasi");
            Console.WriteLine("4. Verilen tarix araligina gore sifarislerin gosterilmesi");
            Console.WriteLine("5. Verilen mebleg araligina gore sifarislerin gosterilmesi");
            Console.WriteLine("6. Verilmis bir tarixde olan sifarislerin gosterilmesi");
            Console.WriteLine("7. Verilmis nomreye esasen hemin nomreli sifarisin melumatlarinin gosterilmesi");
            Console.WriteLine("0. Evvelki menyuya qayit");
            Console.WriteLine();

            var choice = ConsoleHelper.ReadInt("Seciminizi daxil edin", 0, 7);

            switch (choice)
            {
                case 1:
                    await AddOrderAsync();
                    break;
                case 2:
                    await RemoveOrderAsync();
                    break;
                case 3:
                    await ShowAllOrdersAsync();
                    break;
                case 4:
                    await ShowOrdersByDateRangeAsync();
                    break;
                case 5:
                    await ShowOrdersByPriceRangeAsync();
                    break;
                case 6:
                    await ShowOrdersBySpecificDateAsync();
                    break;
                case 7:
                    await ShowOrderDetailsByNoAsync();
                    break;
                case 0:
                    return;
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }
    }

    private async Task AddOrderAsync()
    {
        ConsoleHelper.PrintSubHeader("Yeni Sifaris Elave Etmek");

        var menuItems = (await _menuItemService.GetAllAsync()).ToList();
        if (!menuItems.Any())
        {
            ConsoleHelper.PrintError("Menyuda hec bir mehsul yoxdur! Evvelce menyuya mehsul elave edin.");
            return;
        }

        Console.WriteLine("\n--- Menyudaki Mehsullar ---");
        Console.WriteLine("{0,-6} | {1,-25} | {2,-15} | {3,-10}", "ID", "Ad", "Kateqoriya", "Qiymet");
        Console.WriteLine(new string('-', 62));
        foreach (var m in menuItems)
        {
            Console.WriteLine("{0,-6} | {1,-25} | {2,-15} | {3,-10:F2} AZN", m.Id, m.Name, m.CategoryName, m.Price);
        }
        Console.WriteLine(new string('-', 62));

        var orderItems = new List<OrderItemCreateDto>();

        while (true)
        {
            Console.WriteLine("\nSifarise mehsul elave etmek ucun Menu Item ID daxil edin (Sifarisi tamamlamaq ucun 0 daxil edin):");
            var menuItemId = ConsoleHelper.ReadInt("Menu Item ID", 0);

            if (menuItemId == 0)
            {
                if (!orderItems.Any())
                {
                    ConsoleHelper.PrintInfo("Sifaris legv edildi (hec bir mehsul secilmedi).");
                    return;
                }
                break;
            }

            var menuItem = menuItems.FirstOrDefault(m => m.Id == menuItemId);
            if (menuItem == null)
            {
                ConsoleHelper.PrintError($"#{menuItemId} nomreli menu mehsulu tapilmadi!");
                continue;
            }

            var count = ConsoleHelper.ReadInt($"'{menuItem.Name}' mehsulunun sayini daxil edin", 1, 1000);

            var existingItem = orderItems.FirstOrDefault(oi => oi.MenuItemId == menuItemId);
            if (existingItem != null)
            {
                existingItem.Count += count;
                ConsoleHelper.PrintInfo($"'{menuItem.Name}' sayi artirildi. Cari cemi say: {existingItem.Count}");
            }
            else
            {
                orderItems.Add(new OrderItemCreateDto
                {
                    MenuItemId = menuItemId,
                    Count = count
                });
                ConsoleHelper.PrintSuccess($"'{menuItem.Name}' x {count} sifarise elave edildi.");
            }
        }

        try
        {
            var createdOrder = await _orderService.AddOrderAsync(new OrderCreateDto { Items = orderItems });
            ConsoleHelper.PrintSuccess($"\nSifaris ugurla yaradildi!");
            Console.WriteLine($"Sifaris No: #{createdOrder.Id}");
            Console.WriteLine($"Tarix: {createdOrder.Date:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Umumi mehsul sayi: {createdOrder.TotalItemCount}");
            Console.WriteLine($"Umumi mebleg: {createdOrder.TotalAmount:F2} AZN");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task RemoveOrderAsync()
    {
        ConsoleHelper.PrintSubHeader("Sifarisin Legvi");

        var orderId = ConsoleHelper.ReadInt("Legv edilecek sifaris nomresi (ID)", 1);

        try
        {
            await _orderService.RemoveOrderAsync(orderId);
            ConsoleHelper.PrintSuccess($"#{orderId} nomreli sifaris ugurla legv edildi (silindi)!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task ShowAllOrdersAsync()
    {
        ConsoleHelper.PrintSubHeader("Butun Sifarisler");
        var orders = await _orderService.GetAllOrdersAsync();
        PrintOrdersTable(orders);
    }

    private async Task ShowOrdersByDateRangeAsync()
    {
        ConsoleHelper.PrintSubHeader("Verilen Tarix Araligina Gore Sifarisler");

        var startDate = ConsoleHelper.ReadDate("Baslangic tarixi");
        var endDate = ConsoleHelper.ReadDate("Son tarix");

        try
        {
            var orders = await _orderService.GetOrdersByDatesIntervalAsync(startDate, endDate);
            PrintOrdersTable(orders);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task ShowOrdersByPriceRangeAsync()
    {
        ConsoleHelper.PrintSubHeader("Verilen Mebleg Araligina Gore Sifarisler");

        var minPrice = ConsoleHelper.ReadDecimal("Minimum mebleg (AZN)", 0);
        var maxPrice = ConsoleHelper.ReadDecimal("Maximum mebleg (AZN)", minPrice);

        try
        {
            var orders = await _orderService.GetOrdersByPriceIntervalAsync(minPrice, maxPrice);
            PrintOrdersTable(orders);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task ShowOrdersBySpecificDateAsync()
    {
        ConsoleHelper.PrintSubHeader("Verilmis Bir Tarixde Olan Sifarisler");

        var date = ConsoleHelper.ReadDate("Tarixi daxil edin");

        var orders = await _orderService.GetOrdersByDateAsync(date);
        PrintOrdersTable(orders);
    }

    private async Task ShowOrderDetailsByNoAsync()
    {
        ConsoleHelper.PrintSubHeader("Sifaris Melumatlari (Nomreye esasen)");

        var orderId = ConsoleHelper.ReadInt("Sifaris nomresini daxil edin (ID)", 1);

        var order = await _orderService.GetOrderByNoAsync(orderId);
        if (order == null)
        {
            ConsoleHelper.PrintError($"#{orderId} nomreli sifaris tapilmadi!");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Sifaris Nomresi:   #{order.Id}");
        Console.WriteLine($"Tarix:             {order.Date:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Cemi Mehsul Sayi:  {order.TotalItemCount}");
        Console.WriteLine($"Umumi Mebleg:      {order.TotalAmount:F2} AZN");
        Console.WriteLine();

        Console.WriteLine("--- Sifaris Item-lari ---");
        Console.WriteLine("{0,-6} | {1,-25} | {2,-8} | {3,-12} | {4,-12}", "No", "Mehsul Adi", "Sayi", "Vahid Qiymet", "Cemi");
        Console.WriteLine(new string('-', 72));

        int i = 1;
        foreach (var item in order.OrderItems)
        {
            Console.WriteLine("{0,-6} | {1,-25} | {2,-8} | {3,-12:F2} | {4,-12:F2} AZN",
                i++, item.MenuItemName, item.Count, item.UnitPrice, item.SubTotal);
        }
        Console.WriteLine(new string('-', 72));
    }

    private static void PrintOrdersTable(IEnumerable<OrderDto> orders)
    {
        var list = orders.ToList();
        if (!list.Any())
        {
            ConsoleHelper.PrintInfo("Hec bir sifaris tapilmadi.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("{0,-8} | {1,-14} | {2,-16} | {3,-20}", "Sifaris No", "Meblegi (AZN)", "Menu Item Sayi", "Tarixi");
        Console.WriteLine(new string('-', 68));

        foreach (var o in list)
        {
            Console.WriteLine("{0,-8} | {1,-14:F2} | {2,-16} | {3,-20:yyyy-MM-dd HH:mm:ss}",
                $"#{o.Id}", o.TotalAmount, o.TotalItemCount, o.Date);
        }
        Console.WriteLine(new string('-', 68));
        Console.WriteLine($"Cemi Sifaris Sayi: {list.Count} | Umumi Dövriyyə: {list.Sum(x => x.TotalAmount):F2} AZN");
    }
}
