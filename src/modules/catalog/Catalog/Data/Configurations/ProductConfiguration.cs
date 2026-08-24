using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Products.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // 1. Table Name & Primary Key
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        // 2. Simple Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.ImageFile)
            .HasMaxLength(255);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)"); // Crucial for precise monetary values

        // 3. Encapsulated Collection Mapping
        builder.Property(p => p.Category).IsRequired();

    }
}