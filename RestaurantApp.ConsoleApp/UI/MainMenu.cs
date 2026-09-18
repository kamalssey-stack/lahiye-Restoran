using RestaurantApp.ConsoleApp.UI;

namespace RestaurantApp.ConsoleApp.UI;

public class MainMenu
{
    private readonly MenuItemMenu _menuItemMenu;
    private readonly OrderMenu _orderMenu;

    public MainMenu(MenuItemMenu menuItemMenu, OrderMenu orderMenu)
    {
        _menuItemMenu = menuItemMenu;
        _orderMenu = orderMenu;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader("RESTORAN SIFARISLERININ IDARE EDILMESI SISTEMI");
            Console.WriteLine("1. Menu uzerinde emeliyyat aparmaq");
            Console.WriteLine("2. Sifarisler uzerinde emeliyyat aparmaq");
            Console.WriteLine("0. Sistemden cixmaq");
            Console.WriteLine();

            var choice = ConsoleHelper.ReadInt("Seciminizi daxil edin", 0, 2);

            switch (choice)
            {
                case 1:
                    await _menuItemMenu.ShowAsync();
                    break;
                case 2:
                    await _orderMenu.ShowAsync();
                    break;
                case 0:
                    Console.WriteLine("\nSistemden cixilir... Tesekkur edirik!");
                    return;
            }
        }
    }
}
