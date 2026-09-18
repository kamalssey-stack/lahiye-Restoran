using System.Globalization;

namespace RestaurantApp.ConsoleApp.UI;

public static class ConsoleHelper
{
    public static void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('=', 65));
        Console.WriteLine($"  {title.ToUpper(CultureInfo.CurrentCulture)}");
        Console.WriteLine(new string('=', 65));
        Console.ResetColor();
    }

    public static void PrintSubHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"--- {title} ---");
        Console.ResetColor();
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[✓] {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[✗] Xəta: {message}");
        Console.ResetColor();
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"[i] {message}");
        Console.ResetColor();
    }

    public static string ReadString(string prompt, bool required = true)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                if (!required) return string.Empty;
                PrintError("Bu xana boş buraxıla bilməz!");
                continue;
            }
            return input.Trim();
        }
    }

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out int result) && result >= min && result <= max)
            {
                return result;
            }
            PrintError($"Zəhmət olmasa {min} ilə {max} arasında düzgün tam ədəd daxil edin!");
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min = 0, decimal max = decimal.MaxValue)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine()?.Replace(',', '.');
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) && result >= min && result <= max)
            {
                return result;
            }
            PrintError($"Zəhmət olmasa {min} ilə {max} arasında düzgün məbləğ daxil edin (məs: 5.50)!");
        }
    }

    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (format: yyyy-MM-dd və ya dd.MM.yyyy): ");
            var input = Console.ReadLine();
            string[] formats = { "yyyy-MM-dd", "dd.MM.yyyy", "dd/MM/yyyy", "yyyy/MM/dd" };
            if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            if (DateTime.TryParse(input, out date))
            {
                return date;
            }
            PrintError("Tarix formatı düzgün deyil! Nümunə: 2026-09-18 və ya 18.09.2026");
        }
    }

    public static void PressAnyKeyToContinue()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Davam etmək üçün hər hansı bir düyməyə basın...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
