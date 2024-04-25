namespace Modules.Ordering.Infrastructure.EntityFramework;

using BridgingIT.DevKit.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Modules.Ordering.Domain;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.Property(e => e.Id)
            .HasValueGenerator<GuidValueGenerator>().ValueGeneratedOnAdd();

        builder.Ignore(e => e.DomainEvents);

        builder.OwnsOne(e => e.Address, o =>
            {
                o.WithOwner();

                o.Property(e => e.Line1)
                    .HasMaxLength(256)
                    .IsRequired();

                o.Property(e => e.Line2)
                    .HasMaxLength(256);

                o.Property(e => e.City)
                    .HasMaxLength(128)
                    .IsRequired();

                o.Property(e => e.Country)
                    .HasMaxLength(128)
                    .IsRequired();

                o.Property(e => e.PostalCode)
                    .HasMaxLength(32)
                    .IsRequired();
            });

        builder.Property(e => e.BuyerId)
            .IsRequired(false);

        builder.Property(e => e.CreatedDate)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(1024)
            .IsRequired(false);

        builder.Property(p => p.OrderStatus)
               .IsRequired()
               .HasConversion<EnumerationConverter<OrderStatus>>();
        //.HasConversion(
        //     v => v.Id,
        //     v => Enumeration.From<OrderStatus>(v)); // TODO: use EnumerationConverter

        builder.Property(e => e.PaymentMethodId)
            .IsRequired(false);

        builder.HasMany(e => e.Items)
           .WithOne()
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);
    }
}