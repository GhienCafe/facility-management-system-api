using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class ReplacementDetail : BaseEntity
{
    public Guid ReplacementId { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ReplacementAssetId { get; set; }
    public bool IsDone { get; set; }
    public string? Note { get; set; }
    
    public virtual Replacement? Replacement { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class ReplacementDetailConfig : IEntityTypeConfiguration<ReplacementDetail>
{
    public void Configure(EntityTypeBuilder<ReplacementDetail> builder)
    {
        builder.ToTable("ReplacementDetails");
        builder.Property(x => x.ReplacementId).IsRequired();
        builder.Property(x => x.AssetId).IsRequired(false);
        builder.Property(x => x.ReplacementAssetId).IsRequired(false);
        builder.Property(x => x.Note).IsRequired(false);
        builder.Property(x => x.IsDone).IsRequired();
   
        //Relationship
        builder.HasOne(x => x.Replacement)
            .WithMany(x => x.ReplacementDetails)
            .HasForeignKey(x => x.ReplacementId);

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.ReplacementDetails)
            .HasForeignKey(x => x.AssetId);

    }
}

