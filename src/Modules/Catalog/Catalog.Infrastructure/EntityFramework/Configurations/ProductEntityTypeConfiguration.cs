namespace Modules.Catalog.Infrastructure.EntityFramework;

using BridgingIT.DevKit.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Modules.Catalog.Domain;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        var valueComparer = new ValueComparer<IEnumerable<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.ToTable(typeof(Product).Name);

        builder.Property(e => e.Id)
            .HasValueGenerator<GuidValueGenerator>().ValueGeneratedOnAdd();

        //builder.HasQueryFilter(e => e.State.Deleted == null || e.State.Deleted == false);

        builder.Ignore(b => b.DomainEvents);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasMaxLength(2048);

        builder.Property(e => e.Barcode)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(e => e.Keywords)
             .HasConversion<StringsSemicolonConverter>()
             .HasMaxLength(8192)
             .Metadata.SetValueComparer(valueComparer);

        builder.Property(e => e.PictureSvg)
            .IsRequired(false)
            .HasMaxLength(8192);

        builder.Property(e => e.PictureFileName)
            .IsRequired(false)
            .HasMaxLength(2048);

        builder.Property(e => e.PictureUri)
            .IsRequired(false)
            .HasMaxLength(2048);

        builder.Property(e => e.Rating)
            .HasDefaultValue(0)
            .IsRequired(true);

        builder.Property(e => e.Price)
            .HasDefaultValue(0)
            .IsRequired(true)
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Sku)
            .IsRequired(true)
            .HasMaxLength(256);

        builder.HasIndex(e => e.Sku)
            .IsUnique();

        builder.Property(e => e.Size)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.HasOne(e => e.Brand)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Type)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsOneAuditState();
    }
}