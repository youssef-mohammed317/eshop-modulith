using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Data.Configurations;

public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
{
    public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
    {
        // 1. Primary Key
        builder.HasKey(i => i.Id);

        // 2. Properties
        builder.Property(i => i.ProductId).IsRequired();

        builder.Property(i => i.Quantity).IsRequired();

        builder.Property(i => i.Color)
            .IsRequired()
            .HasMaxLength(50);

        // Precision for money to prevent rounding errors
        builder.Property(i => i.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(255);
    }
}