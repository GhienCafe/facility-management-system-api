using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Asset : BaseEntity
{
    public string AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public bool IsMovable { get; set; }
    public AssetStatus Status { get; set; }
    public DateTime? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime? LastMaintenanceTime { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    
    //
    public virtual AssetType? Type { get; set; }
    public virtual Model? Model { get; set; }
    public virtual IEnumerable<RoomAsset>? RoomAssets { get; set; }
    public virtual IEnumerable<Maintenance>? Maintenances { get; set; }
    public virtual IEnumerable<Replacement>? Replacements { get; set; }
    public virtual IEnumerable<Transportation>? Transportations { get; set; }
    public virtual IEnumerable<Repairation>? Repairations { get; set; }
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
        
        builder.Property(a => a.TypeId).IsRequired();
        builder.Property(a => a.AssetName).IsRequired();
        builder.Property(a => a.AssetCode).IsRequired(false);
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.IsMovable).IsRequired();
        builder.Property(a => a.ManufacturingYear).IsRequired(false);
        builder.Property(a => a.SerialNumber).IsRequired(false);
        builder.Property(a => a.Quantity).IsRequired();
        builder.Property(a => a.LastMaintenanceTime).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        
        // Attributes
        builder.HasIndex(a => a.AssetCode).IsUnique();

        //Relationship
        builder.HasOne(x => x.Type)
            .WithMany(x => x.Assets)
            .HasForeignKey(x => x.TypeId);
        
        builder.HasOne(x => x.Model)
            .WithMany(x => x.Assets)
            .HasForeignKey(x => x.ModelId);
        
        builder.HasMany(x => x.RoomAssets)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);

        builder.HasMany(x => x.Maintenances)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);

        builder.HasMany(x => x.Replacements)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasMany(x => x.Replacements)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.NewAssetId);

        builder.HasMany(x => x.Transportations)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasMany(x => x.Repairations)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
    }
}   