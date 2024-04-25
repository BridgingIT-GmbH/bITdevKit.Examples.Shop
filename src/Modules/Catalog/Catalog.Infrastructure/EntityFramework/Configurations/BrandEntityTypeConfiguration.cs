namespace Modules.Catalog.Infrastructure.EntityFramework;

using BridgingIT.DevKit.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Modules.Catalog.Domain;

public class BrandEntityTypeConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        var valueComparer = new ValueComparer<IEnumerable<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.ToTable(typeof(Brand).Name);

        builder.Property(e => e.Id)
            .HasValueGenerator<GuidValueGenerator>().ValueGeneratedOnAdd();

        builder.Ignore(b => b.DomainEvents);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasMaxLength(2048);

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

        builder.OwnsOneAuditState();
    }
}