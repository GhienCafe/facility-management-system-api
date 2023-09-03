using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryDetail : BaseEntity
{
    public string? AssetCode { get; set; }
    public Guid InventoryId { get; set; }
    public double? Quantity { get; set; }
    public double? ActualQuantity { get; set; }
    public string? Note { get; set; }
    
    //
    public virtual Inventory? Inventory { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class InventoryDetailConfig : IEntityTypeConfiguration<InventoryDetail>
{
    public void Configure(EntityTypeBuilder<InventoryDetail> builder)
    {
        builder.ToTable("InventoryDetails");
        builder.Property(a => a.AssetCode).IsRequired(false);
        builder.Property(a => a.InventoryId).IsRequired();
        builder.Property(a => a.Quantity).IsRequired(false);
        builder.Property(a => a.ActualQuantity).IsRequired(false);
        builder.Property(a => a.Note).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.Inventory)
            .WithMany(x => x.InventoryDetails)
            .HasForeignKey(x => x.InventoryId);
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.InventoryDetails)
            .HasForeignKey(x => x.AssetCode)
            .HasPrincipalKey(x => x.AssetCode);;
        
    }
}   