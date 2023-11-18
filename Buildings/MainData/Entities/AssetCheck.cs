using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class AssetCheck : BaseRequest
{
    public Guid AssetId { get; set; }
    public bool IsVerified { get; set; }
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class AssetCheckConfig : IEntityTypeConfiguration<AssetCheck>
{
    public void Configure(EntityTypeBuilder<AssetCheck> builder)
    {
        builder.ToTable("AssetChecks");
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.IsVerified).IsRequired().HasDefaultValue(false);
        builder.Property(a => a.CategoryId).IsRequired(false);
        builder.Property(a => a.AssetTypeId).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.AssetChecks)
            .HasForeignKey(x => x.AssignedTo);

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.AssetChecks)
            .HasForeignKey(x => x.AssetId);

        builder.HasMany(x => x.MediaFiles)
            .WithOne(x => x.AssetCheck)
            .HasForeignKey(i => i.AssetCheckId);
    }
}


