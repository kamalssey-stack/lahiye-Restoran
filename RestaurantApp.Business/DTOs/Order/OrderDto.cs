namespace RestaurantApp.Business.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public int TotalItemCount { get; set; }
}
