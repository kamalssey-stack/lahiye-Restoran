namespace RestaurantApp.Business.DTOs.Order;

public class OrderCreateDto
{
    public List<OrderItemCreateDto> Items { get; set; } = new();
}
