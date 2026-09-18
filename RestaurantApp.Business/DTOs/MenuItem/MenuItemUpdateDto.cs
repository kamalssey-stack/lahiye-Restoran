namespace RestaurantApp.Business.DTOs.MenuItem;

public class MenuItemUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int? CategoryId { get; set; }
}
