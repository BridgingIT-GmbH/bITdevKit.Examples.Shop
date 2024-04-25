namespace Modules.Shopping.Infrastructure.EntityFramework;

using BridgingIT.DevKit.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Shopping.Domain;
using Modules.Shopping.Infrastructure.EntityFramework.Configurations.Converters;

public class CartEntityTypeConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        ConfigureCart(builder);
        ConfigureCartItems(builder);
    }

    private static void ConfigureCart(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(v => v.Value, v => new CartId(v));

        builder.Ignore(e => e.DomainEvents);

        builder.Property(e => e.Identity)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(nameof(Cart.Identity))
          .IsUnique(true);

        builder.Property(e => e.TotalPrice)
            .IsRequired()
            //.HasDefaultValue(0m)
            .HasColumnType("decimal(18, 2)")
            .HasConversion<AmountConverter>();

        builder.OwnsOneAuditState();
    }

    private static void ConfigureCartItems(EntityTypeBuilder<Cart> builder)
    {
        builder.OwnsMany(m => m.Items, ib =>
        {
            ib.ToTable("CartItems");

            ib.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasConversion(v => v.Value, v => new CartItemId(v));

            ib.Property(e => e.Quantity)
                .IsRequired();

            ib.Property(e => e.TotalPrice)
                .IsRequired()
                //.HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasConversion<AmountConverter>();

            ib.OwnsOne(e => e.Product, o =>
            {
                o.Property(b => b.SKU)
                    .HasColumnName(nameof(CartProduct.SKU))
                    .HasMaxLength(256)
                    .IsRequired();

                o.Property(b => b.Name)
                    .HasColumnName(nameof(CartProduct.Name))
                    .HasMaxLength(512)
                    .IsRequired();

                o.Property(b => b.UnitPrice)
                    .HasColumnName(nameof(CartProduct.UnitPrice))
                    .IsRequired()
                    //.HasDefaultValue(0m)
                    .HasColumnType("decimal(18, 2)")
                    .HasConversion<AmountConverter>();
            });
        });

        builder.Metadata.FindNavigation(nameof(Cart.Items))
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}