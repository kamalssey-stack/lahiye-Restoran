using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        builder.HasData(
            new Category { Id = 1, Name = "Şorbalar (Sup)" },
            new Category { Id = 2, Name = "Əsas Yeməklər" },
            new Category { Id = 3, Name = "İçkilər" },
            new Category { Id = 4, Name = "Desertlər" },
            new Category { Id = 5, Name = "Qəlyanaltılar" }
        );
    }
}
