using RestaurantApp.Business.DTOs.MenuItem;
using RestaurantApp.Business.DTOs.Order;
using RestaurantApp.Business.Services.Interfaces;

namespace RestaurantApp.ConsoleApp.UI;

public class DemoRunner
{
    private readonly ICategoryService _categoryService;
    private readonly IMenuItemService _menuItemService;
    private readonly IOrderService _orderService;

    public DemoRunner(ICategoryService categoryService, IMenuItemService menuItemService, IOrderService orderService)
    {
        _categoryService = categoryService;
        _menuItemService = menuItemService;
        _orderService = orderService;
    }

    public async Task RunDemoAsync()
    {
        ConsoleHelper.PrintHeader("DATA BAZANIN YOXLANILMASI VE DEMO EMELIYYATLAR");

        // 1. Kategoriyalar
        ConsoleHelper.PrintSubHeader("1. Bazadakı Kateqoriyalar");
        var categories = await _categoryService.GetAllAsync();
        foreach (var c in categories)
        {
            Console.WriteLine($"  ID: {c.Id} | Kateqoriya: {c.Name}");
        }

        // 2. Bütün menyu məhsulları
        ConsoleHelper.PrintSubHeader("2. Bazadakı Menyu Məhsulları (AutoMapper & Cache)");
        var menuItems = (await _menuItemService.GetAllAsync()).ToList();
        foreach (var m in menuItems)
        {
            Console.WriteLine($"  [{m.Id}] {m.Name,-20} | {m.CategoryName,-15} | {m.Price:F2} AZN");
        }

        // 3. Kateqoriyaya görə axtarış (İçkilər - ID 3)
        ConsoleHelper.PrintSubHeader("3. 'İçkilər' Kateqoriyasındakı Məhsullar (ID 3)");
        var drinks = await _menuItemService.GetByCategoryAsync(3);
        foreach (var d in drinks)
        {
            Console.WriteLine($"  [{d.Id}] {d.Name} - {d.Price:F2} AZN");
        }

        // 4. Qiymət aralığına görə (5.00 - 10.00 AZN)
        ConsoleHelper.PrintSubHeader("4. Qiyməti 5.00 AZN - 10.00 AZN arasında olan məhsullar");
        var priceFilter = await _menuItemService.GetByPriceIntervalAsync(5.00m, 10.00m);
        foreach (var item in priceFilter)
        {
            Console.WriteLine($"  [{item.Id}] {item.Name} - {item.Price:F2} AZN");
        }

        // 5. Ada görə axtarış (məs: 'Kabab')
        ConsoleHelper.PrintSubHeader("5. Ada görə axtarış: 'Kabab'");
        var searchResults = await _menuItemService.SearchByNameAsync("Kabab");
        foreach (var item in searchResults)
        {
            Console.WriteLine($"  [{item.Id}] {item.Name} - {item.Price:F2} AZN");
        }

        // 6. Yeni sifariş yaradılması
        ConsoleHelper.PrintSubHeader("6. Yeni Sifarişin Yaradılması (3 x Lülə Kabab + 2 x Coca-Cola)");
        var lule = menuItems.FirstOrDefault(m => m.Name.Contains("Lülə") || m.Name.Contains("Lule")) ?? menuItems.First();
        var cola = menuItems.FirstOrDefault(m => m.Name.Contains("Cola")) ?? menuItems.Skip(1).First();

        var newOrder = await _orderService.AddOrderAsync(new OrderCreateDto
        {
            Items = new List<OrderItemCreateDto>
            {
                new() { MenuItemId = lule.Id, Count = 3 },
                new() { MenuItemId = cola.Id, Count = 2 }
            }
        });

        ConsoleHelper.PrintSuccess($"Sifariş #{newOrder.Id} yaradıldı!");
        Console.WriteLine($"  Tarix: {newOrder.Date:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"  Məhsul sayı: {newOrder.TotalItemCount} ədəd (3 x {lule.Name} + 2 x {cola.Name})");
        Console.WriteLine($"  Ümumi məbləğ: {newOrder.TotalAmount:F2} AZN");

        // 7. Sifarişin detallı oxunması (GetOrderByNo)
        ConsoleHelper.PrintSubHeader($"7. Sifariş #{newOrder.Id} Detalları (GetOrderByNo)");
        var orderDetail = await _orderService.GetOrderByNoAsync(newOrder.Id);
        if (orderDetail != null)
        {
            foreach (var item in orderDetail.OrderItems)
            {
                Console.WriteLine($"  - {item.MenuItemName}: {item.Count} ədəd x {item.UnitPrice:F2} AZN = {item.SubTotal:F2} AZN");
            }
            Console.WriteLine($"  YEKUN MƏBLƏĞ: {orderDetail.TotalAmount:F2} AZN");
        }

        // 8. Bütün sifarişlərin siyahısı
        ConsoleHelper.PrintSubHeader("8. Bütün Sifarişlərin Siyahısı");
        var allOrders = await _orderService.GetAllOrdersAsync();
        foreach (var ord in allOrders)
        {
            Console.WriteLine($"  Sifariş #{ord.Id} | Cəmi Məbləğ: {ord.TotalAmount:F2} AZN | Say: {ord.TotalItemCount} | Tarix: {ord.Date:yyyy-MM-dd HH:mm:ss}");
        }

        Console.WriteLine();
        ConsoleHelper.PrintSuccess("BÜTÜN DATA BAZA VƏ SERVİS ƏMƏLİYYATLARI UĞURLA İCRA EDİLDİ!");
    }
}
