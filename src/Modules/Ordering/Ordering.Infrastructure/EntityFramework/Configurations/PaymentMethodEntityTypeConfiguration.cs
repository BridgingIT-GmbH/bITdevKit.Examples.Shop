namespace Modules.Ordering.Infrastructure.EntityFramework;

using BridgingIT.DevKit.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Ordering.Domain;

public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentMethods");

        builder.Property(e => e.CardHolderName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.Alias)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.CardNumber)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(e => e.Expiration)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(p => p.CardType)
               .IsRequired()
               .HasConversion<EnumerationConverter<CardType>>();
        //.HasConversion(
        //     v => v.Id,
        //     v => Enumeration.From<CardType>(v)); // TODO: use EnumerationConverter
    }
}
