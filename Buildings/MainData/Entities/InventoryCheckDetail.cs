using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryCheckDetail : BaseEntity
{
    public Guid AssetId { get; set; }
    public Guid InventoryCheckId { get; set; }
    public AssetStatus Status { get; set; }
    
    //Relationship
    public Asset? Asset { get; set; }
    public InventoryCheck? InventoryCheck { get; set; }
}

public class InventoryCheckDetailConfig : IEntityTypeConfiguration<InventoryCheckDetail>
{
    public void Configure(EntityTypeBuilder<InventoryCheckDetail> builder)
    {
        builder.ToTable("InventoryCheckDetails");
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.InventoryCheckId).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        //Relationship

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.InventoryCheckDetails)
            .HasForeignKey(i => i.AssetId);
        
        builder.HasOne(x => x.InventoryCheck)
            .WithMany(x => x.InventoryCheckDetails)
            .HasForeignKey(i => i.AssetId);
    }
}
