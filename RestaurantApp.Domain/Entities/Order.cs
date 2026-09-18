namespace RestaurantApp.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
