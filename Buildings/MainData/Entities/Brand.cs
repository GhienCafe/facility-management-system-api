using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Brand : BaseEntity
{
    public string? BrandName { get; set; }
    public string? Description { get; set; }

    public IEnumerable<Model>? Models { get; set; }
}

public class BrandConfig : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.Property(a => a.BrandName).IsRequired();
        builder.Property(a => a.Description).IsRequired(false);

        builder.HasMany(x => x.Models)
            .WithOne(x => x.Brand)
            .HasForeignKey(x => x.BrandId);
    }
}