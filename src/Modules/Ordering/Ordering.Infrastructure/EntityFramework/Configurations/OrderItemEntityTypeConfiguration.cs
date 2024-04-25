namespace Modules.Ordering.Infrastructure.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Modules.Ordering.Domain;

public partial class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.Property(e => e.Id)
            .HasValueGenerator<GuidValueGenerator>().ValueGeneratedOnAdd();

        builder.Property(e => e.Discount)
            .IsRequired();

        builder.Property(e => e.ProductId)
            .IsRequired();

        builder.Property(e => e.ProductName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.UnitPrice)
            .HasDefaultValue(0)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Discount)
            .HasDefaultValue(0)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Units)
            .IsRequired();

        builder.Property(e => e.PictureUrl)
            .HasMaxLength(512)
            .IsRequired(false);
    }
}
