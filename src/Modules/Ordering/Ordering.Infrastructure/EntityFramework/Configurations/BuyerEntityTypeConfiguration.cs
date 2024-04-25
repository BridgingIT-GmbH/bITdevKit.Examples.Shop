namespace Modules.Ordering.Infrastructure.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Modules.Ordering.Domain;

public class BuyerEntityTypeConfiguration
    : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.ToTable("Buyers");

        builder.Property(e => e.Id)
            .HasValueGenerator<GuidValueGenerator>().ValueGeneratedOnAdd();

        builder.Ignore(b => b.DomainEvents);

        builder.Property(b => b.Identity)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(nameof(Buyer.Identity))
          .IsUnique(true);

        builder.Property(b => b.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(b => b.Email)
            .HasMaxLength(256)
            .HasConversion(
                v => v.Value,
                v => EmailAddress.For(v))
            .IsRequired();

        builder.HasMany(b => b.PaymentMethods)
           .WithOne()
           //.IsRequired()
           .OnDelete(DeleteBehavior.Cascade);
    }
}