using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Count).IsRequired();
        builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2).IsRequired();

        builder.HasOne(oi => oi.MenuItem)
               .WithMany(m => m.OrderItems)
               .HasForeignKey(oi => oi.MenuItemId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
