using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Asset : BaseEntity
{
    public Guid CategoryId { get; set; }
    public string AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public AssetStatus Status { get; set; }
    public DateTime ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime? LastMaintenanceTime { get; set; }
    
    //
    public virtual AssetCategory? AssetCategory { get; set; }
    public virtual IEnumerable<RoomAsset>? RoomAssets { get; set; }
    public virtual IEnumerable<HandoverDetail>? HandoverDetails { get; set; }
    public virtual IEnumerable<MaintenanceDetail>? MaintenanceDetails { get; set; }
    public virtual IEnumerable<InventoryDetail>? InventoryDetails { get; set; }
    
}

public enum AssetStatus
{
    Operational = 1,
    Inactive = 2,
    Maintenance = 3,
    Repair = 4,
    Disposed = 5,
    OutOfOrder = 6,
    Pending = 7,
    NotAvailable = 8,
    NeedInspection = 9,
    Upgraded = 10
}

public class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");
        
        builder.Property(a => a.CategoryId).IsRequired();
        builder.Property(a => a.AssetName).IsRequired();
        builder.Property(a => a.AssetCode).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.ManufacturingYear).IsRequired();
        builder.Property(a => a.SerialNumber).IsRequired(false);
        builder.Property(a => a.Quantity).IsRequired();
        builder.Property(a => a.LastMaintenanceTime).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        
        // Attributes
        builder.HasIndex(a => a.AssetCode).IsUnique();

        //Relationship
        builder.HasOne(x => x.AssetCategory)
            .WithMany(x => x.Assets)
            .HasForeignKey(x => x.CategoryId);
        
        builder.HasMany(x => x.RoomAssets)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasMany(x => x.HandoverDetails)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetCode) 
            .HasPrincipalKey(x => x.AssetCode); 
        
        builder.HasMany(x => x.MaintenanceDetails)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetCode) 
            .HasPrincipalKey(x => x.AssetCode); 

        builder.HasMany(x => x.InventoryDetails)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetCode) 
            .HasPrincipalKey(x => x.AssetCode); 
    }
}   