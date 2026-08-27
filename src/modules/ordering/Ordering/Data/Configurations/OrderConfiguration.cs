using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Ordering.Models;
using Ordering.Ordering.Enums;

namespace Ordering.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId).IsRequired();

        builder.Property(o => o.OrderName)
            .IsRequired()
            .HasMaxLength(100);

        // 1. Map Enum as a string (e.g., "Pending", "Completed") instead of an integer
        builder.Property(o => o.OrderStatus)
            .HasConversion(
                v => v.ToString(),
                v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v))
            .HasMaxLength(50);

        // 2. Ignore Computed Property
        builder.Ignore(o => o.TotalPrice);

        // 3. Encapsulate the One-to-Many Relationship
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // Deleting the order deletes its items

        builder.Metadata.FindNavigation(nameof(Order.OrderItems))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // 4. Map Value Objects (EF Core 8+ Complex Types or Owned Entities)
        builder.ComplexProperty(o => o.ShippingAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
            addressBuilder.Property(a => a.LastName).HasMaxLength(50).IsRequired();
            addressBuilder.Property(a => a.EmailAddress).HasMaxLength(100);
            addressBuilder.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
            addressBuilder.Property(a => a.Country).HasMaxLength(50);
            addressBuilder.Property(a => a.State).HasMaxLength(50);
            addressBuilder.Property(a => a.ZipCode).HasMaxLength(10).IsRequired();
        });

        builder.ComplexProperty(o => o.BillingAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
            addressBuilder.Property(a => a.LastName).HasMaxLength(50).IsRequired();
            addressBuilder.Property(a => a.EmailAddress).HasMaxLength(100);
            addressBuilder.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
            addressBuilder.Property(a => a.Country).HasMaxLength(50);
            addressBuilder.Property(a => a.State).HasMaxLength(50);
            addressBuilder.Property(a => a.ZipCode).HasMaxLength(10).IsRequired();
        });

        builder.ComplexProperty(o => o.Payment, paymentBuilder =>
        {
            paymentBuilder.Property(p => p.CardName).HasMaxLength(100).IsRequired();
            paymentBuilder.Property(p => p.CardNumber).HasMaxLength(24).IsRequired();
            paymentBuilder.Property(p => p.Expiration).HasMaxLength(10).IsRequired();
            paymentBuilder.Property(p => p.Cvv).HasMaxLength(4).IsRequired();
            paymentBuilder.Property(p => p.PaymentMethod).IsRequired();
        });
    }
}