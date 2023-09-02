using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class RoomAsset : BaseEntity
{
    public Guid RoomId { get; set; }
    public Guid AssetId { get; set; }
    public AssetStatus Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    //
    public virtual Room? Room { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class RoomAssetConfig : IEntityTypeConfiguration<RoomAsset>
{
    public void Configure(EntityTypeBuilder<RoomAsset> builder)
    {
        builder.ToTable("RoomAssets");
        builder.Property(a => a.RoomId).IsRequired();
        builder.Property(a => a.AssetId).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.FromDate).IsRequired(false);
        builder.Property(a => a.ToDate).IsRequired(false);
        
        //Relationship
        builder.HasOne(x => x.Room)
            .WithMany(x => x.RoomAssets)
            .HasForeignKey(x => x.RoomId);
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.RoomAssets)
            .HasForeignKey(x => x.AssetId);
    }
}
