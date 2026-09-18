using RestaurantApp.Business.DTOs.MenuItem;
using RestaurantApp.Business.Services.Interfaces;

namespace RestaurantApp.ConsoleApp.UI;

public class MenuItemMenu
{
    private readonly IMenuItemService _menuItemService;
    private readonly ICategoryService _categoryService;

    public MenuItemMenu(IMenuItemService menuItemService, ICategoryService categoryService)
    {
        _menuItemService = menuItemService;
        _categoryService = categoryService;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader("MENYU MEHSULLARI EMELIYYATLARI");
            Console.WriteLine("1. Yeni item elave et");
            Console.WriteLine("2. Item uzerinde duzelis et");
            Console.WriteLine("3. Item sil");
            Console.WriteLine("4. Butun Item-lari goster");
            Console.WriteLine("5. Categoriyasina gore menu item-lari goster");
            Console.WriteLine("6. Qiymet araligina gore menu item-lar goster");
            Console.WriteLine("7. Menu item-lar arasinda ada gore axtaris et (search)");
            Console.WriteLine("0. Evvelki menyuya qayit");
            Console.WriteLine();

            var choice = ConsoleHelper.ReadInt("Seciminizi daxil edin", 0, 7);

            switch (choice)
            {
                case 1:
                    await AddMenuItemAsync();
                    break;
                case 2:
                    await EditMenuItemAsync();
                    break;
                case 3:
                    await RemoveMenuItemAsync();
                    break;
                case 4:
                    await ShowAllMenuItemsAsync();
                    break;
                case 5:
                    await ShowByCategoryAsync();
                    break;
                case 6:
                    await ShowByPriceRangeAsync();
                    break;
                case 7:
                    await SearchByNameAsync();
                    break;
                case 0:
                    return;
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }
    }

    private async Task AddMenuItemAsync()
    {
        ConsoleHelper.PrintSubHeader("Yeni Menu Item Elave Et");

        var categories = (await _categoryService.GetAllAsync()).ToList();
        if (!categories.Any())
        {
            ConsoleHelper.PrintError("Sistemde hec bir kateqoriya tapilmadi!");
            return;
        }

        Console.WriteLine("\nKateqoriyalar:");
        foreach (var cat in categories)
        {
            Console.WriteLine($"  [{cat.Id}] {cat.Name}");
        }
        Console.WriteLine();

        var categoryId = ConsoleHelper.ReadInt("Kateqoriya Id-sini secin", categories.Min(c => c.Id), categories.Max(c => c.Id));
        var name = ConsoleHelper.ReadString("Mehsulun adi");
        var price = ConsoleHelper.ReadDecimal("Mehsulun qiymeti (AZN)", 0.01m, 10000m);

        try
        {
            var created = await _menuItemService.AddAsync(new MenuItemCreateDto
            {
                Name = name,
                Price = price,
                CategoryId = categoryId
            });

            ConsoleHelper.PrintSuccess($"Menyu mehsulu ugurla elave edildi! (ID: {created.Id}, Ad: {created.Name}, Qiymet: {created.Price:F2} AZN, Kateqoriya: {created.CategoryName})");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task EditMenuItemAsync()
    {
        ConsoleHelper.PrintSubHeader("Item Uzerinde Duzelis Et");

        var id = ConsoleHelper.ReadInt("Duzelis edilecek menuItem nomresi (ID)", 1);
        var existing = await _menuItemService.GetByIdAsync(id);
        if (existing == null)
        {
            ConsoleHelper.PrintError($"#{id} nomreli menu item tapilmadi!");
            return;
        }

        Console.WriteLine($"Cari melumatlar: Ad: {existing.Name}, Qiymet: {existing.Price:F2} AZN, Kateqoriya: {existing.CategoryName}");

        var newName = ConsoleHelper.ReadString("Yeni ad");
        var newPrice = ConsoleHelper.ReadDecimal("Yeni qiymet (AZN)", 0.01m, 10000m);

        try
        {
            await _menuItemService.EditAsync(id, new MenuItemUpdateDto
            {
                Name = newName,
                Price = newPrice
            });

            ConsoleHelper.PrintSuccess($"#{id} nomreli menu mehsulu ugurla yenilendi!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task RemoveMenuItemAsync()
    {
        ConsoleHelper.PrintSubHeader("Item Sil");

        var id = ConsoleHelper.ReadInt("Silinecek item nomresi (ID)", 1);

        try
        {
            await _menuItemService.RemoveAsync(id);
            ConsoleHelper.PrintSuccess($"#{id} nomreli menu mehsulu ugurla silindi!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task ShowAllMenuItemsAsync()
    {
        ConsoleHelper.PrintSubHeader("Butun Menyu Mehsullari");
        var items = await _menuItemService.GetAllAsync();
        PrintMenuItemsTable(items);
    }

    private async Task ShowByCategoryAsync()
    {
        ConsoleHelper.PrintSubHeader("Kateqoriyasina Gore Menu Item-lar");

        var categories = (await _categoryService.GetAllAsync()).ToList();
        if (!categories.Any())
        {
            ConsoleHelper.PrintError("Sistemde hec bir kateqoriya tapilmadi!");
            return;
        }

        Console.WriteLine("Movcud Kateqoriyalar:");
        foreach (var cat in categories)
        {
            Console.WriteLine($"  [{cat.Id}] {cat.Name}");
        }
        Console.WriteLine();

        var categoryId = ConsoleHelper.ReadInt("Kateqoriya nomresini secin", categories.Min(c => c.Id), categories.Max(c => c.Id));
        var items = await _menuItemService.GetByCategoryAsync(categoryId);
        PrintMenuItemsTable(items);
    }

    private async Task ShowByPriceRangeAsync()
    {
        ConsoleHelper.PrintSubHeader("Qiymet Araligina Gore Menu Item-lar");

        var minPrice = ConsoleHelper.ReadDecimal("Minimum qiymet", 0);
        var maxPrice = ConsoleHelper.ReadDecimal("Maximum qiymet", minPrice);

        try
        {
            var items = await _menuItemService.GetByPriceIntervalAsync(minPrice, maxPrice);
            PrintMenuItemsTable(items);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }
    }

    private async Task SearchByNameAsync()
    {
        ConsoleHelper.PrintSubHeader("Ada Gore Axtaris (Search)");

        var searchText = ConsoleHelper.ReadString("Axtaris metnini daxil edin");
        var items = await _menuItemService.SearchByNameAsync(searchText);
        PrintMenuItemsTable(items);
    }

    private static void PrintMenuItemsTable(IEnumerable<MenuItemDto> items)
    {
        var list = items.ToList();
        if (!list.Any())
        {
            ConsoleHelper.PrintInfo("Hec bir menu mehsulu tapilmadi.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("{0,-6} | {1,-25} | {2,-18} | {3,-12}", "Nomre", "Adi", "Kateqoriyasi", "Qiymeti (AZN)");
        Console.WriteLine(new string('-', 68));

        foreach (var item in list)
        {
            Console.WriteLine("{0,-6} | {1,-25} | {2,-18} | {3,-12:F2}", item.Id, item.Name, item.CategoryName, item.Price);
        }
        Console.WriteLine(new string('-', 68));
        Console.WriteLine($"Cemi: {list.Count} mehsul");
    }
}
