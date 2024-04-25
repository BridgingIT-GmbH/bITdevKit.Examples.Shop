namespace Modules.Shopping.Infrastructure.EntityFramework;

using BridgingIT.DevKit.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Modules.Shopping.Domain;

public class ShoppingDbContext : DbContext, IDocumentStoreContext
{
    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cart> Carts { get; set; }

    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<StorageDocument> StorageDocuments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuted, LogLevel.Debug)));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(
            this.GetType().Name.ToLowerInvariant().Replace("dbcontext", string.Empty, StringComparison.OrdinalIgnoreCase));

        modelBuilder.ApplyConfiguration(new CartEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TenantEntityTypeConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}