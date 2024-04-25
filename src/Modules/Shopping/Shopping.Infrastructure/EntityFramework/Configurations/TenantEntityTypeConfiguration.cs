namespace Modules.Shopping.Infrastructure.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Shopping.Domain;

public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        ConfigureTenant(builder);
        ConfigureTenantCarts(builder);
    }

    private static void ConfigureTenant(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(v => v.Value, v => new TenantId(v));

        builder.Property(e => e.Name)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }

    private static void ConfigureTenantCarts(EntityTypeBuilder<Tenant> builder)
    {
        builder.OwnsMany(h => h.CartIds, cib =>
        {
            cib.ToTable("TenantCarts");
            cib.HasKey("Id");
            cib.WithOwner()
                .HasForeignKey("TenantId");
            cib.Property(ci => ci.Value)
                .HasColumnName("CartId");
        });

        builder.Metadata.FindNavigation(nameof(Tenant.CartIds))
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}