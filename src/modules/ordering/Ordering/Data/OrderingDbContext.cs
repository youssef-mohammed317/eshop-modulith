using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Ordering.Ordering.Models;

namespace Ordering.Data;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. Set the default schema for this microservice
        builder.HasDefaultSchema("ordering");

        // 2. Automatically apply all IEntityTypeConfiguration classes in this assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}