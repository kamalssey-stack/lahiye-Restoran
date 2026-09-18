namespace RestaurantApp.Business.DTOs.Order;

public class OrderItemDetailDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal => Count * UnitPrice;
}
