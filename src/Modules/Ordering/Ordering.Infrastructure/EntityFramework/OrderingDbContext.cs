namespace Modules.Ordering.Infrastructure.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Modules.Ordering.Domain;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Buyer> Buyers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuted, LogLevel.Debug)));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(
            this.GetType().Name.ToLowerInvariant().Replace("dbcontext", string.Empty, StringComparison.OrdinalIgnoreCase));

        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
