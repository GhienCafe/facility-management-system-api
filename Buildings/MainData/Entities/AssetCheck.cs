using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class AssetCheck : BaseEntity
{
    public Guid RequestId { get; set; }
    public Guid AssetId { get; set; }
    public bool IsVerified { get; set; }
    
    // Relationship
    public virtual ActionRequest? Request { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class AssetCheckConfig : IEntityTypeConfiguration<AssetCheck>
{
    public void Configure(EntityTypeBuilder<AssetCheck> builder)
    {
        builder.ToTable("AssetChecks");
        builder.Property(x => x.RequestId).IsRequired();
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.IsVerified).IsRequired(false);

        //Relationship

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.AssetChecks)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasOne(x => x.Request)
            .WithMany(x => x.AssetChecks)
            .HasForeignKey(x => x.RequestId);

    }
}


