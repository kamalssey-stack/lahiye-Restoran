using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(150);
        builder.HasIndex(m => m.Name).IsUnique();
        builder.Property(m => m.Price).HasPrecision(18, 2).IsRequired();

        builder.HasOne(m => m.Category)
               .WithMany(c => c.MenuItems)
               .HasForeignKey(m => m.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new MenuItem { Id = 1, Name = "Mərci Şorbası", Price = 4.50m, CategoryId = 1 },
            new MenuItem { Id = 2, Name = "Düşbərə", Price = 6.00m, CategoryId = 1 },
            new MenuItem { Id = 3, Name = "Lülə Kabab", Price = 12.00m, CategoryId = 2 },
            new MenuItem { Id = 4, Name = "Tikə Kabab", Price = 14.00m, CategoryId = 2 },
            new MenuItem { Id = 5, Name = "Toyuq Şnitsel", Price = 9.50m, CategoryId = 2 },
            new MenuItem { Id = 6, Name = "Coca-Cola 0.5L", Price = 2.50m, CategoryId = 3 },
            new MenuItem { Id = 7, Name = "Ayran", Price = 1.50m, CategoryId = 3 },
            new MenuItem { Id = 8, Name = "Təbii Şirə", Price = 3.00m, CategoryId = 3 },
            new MenuItem { Id = 9, Name = "Çizkeyk", Price = 7.00m, CategoryId = 4 },
            new MenuItem { Id = 10, Name = "Paxlava", Price = 5.00m, CategoryId = 4 }
        );
    }
}
