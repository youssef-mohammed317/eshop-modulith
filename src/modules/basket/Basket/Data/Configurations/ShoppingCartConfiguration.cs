using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Data.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        // 1. Primary Key
        builder.HasKey(c => c.Id);



        // 2. Properties
        builder.HasIndex(c => c.UserName).IsUnique();

        builder.Property(c => c.UserName)
            .IsRequired()
            .HasMaxLength(100);

        // 3. Ignore computed properties (We don't want a TotalPrice column in the DB)
        builder.Ignore(c => c.TotalPrice);

        // 4. Configure the One-to-Many Relationship
        builder.HasMany(c => c.Items)
               .WithOne()
               .HasForeignKey(ci => ci.ShoppingCartId)
               .OnDelete(DeleteBehavior.Cascade); // Deleting the cart deletes its items

        // 5. DDD Encapsulation: Tell EF Core to use the private backing field for the collection
        builder.Metadata.FindNavigation(nameof(ShoppingCart.Items))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}