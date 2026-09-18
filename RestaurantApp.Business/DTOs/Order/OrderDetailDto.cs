namespace RestaurantApp.Business.DTOs.Order;

public class OrderDetailDto
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public int TotalItemCount { get; set; }
    public List<OrderItemDetailDto> OrderItems { get; set; } = new();
}
