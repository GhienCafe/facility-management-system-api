using System.ComponentModel.DataAnnotations;
using AppCore.Attributes;
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
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime? LastMaintenanceTime { get; set; }
    public DateTime? LastCheckedDate { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public bool? IsRented { get; set; }
    public DateTime? StartDateOfUse { get; set; }
    public string? ImageUrl { get; set; }

    //
    public virtual AssetType? Type { get; set; }
   // public virtual MaintenanceScheduleConfig? MaintenanceScheduleConfigs { get; set; }
    public virtual Model? Model { get; set; }
    public virtual IEnumerable<RoomAsset>? RoomAssets { get; set; }
    public virtual IEnumerable<Maintenance>? Maintenances { get; set; }
    public virtual IEnumerable<Replacement>? Replacements { get; set; }
    public virtual ICollection<TransportationDetail>? TransportationDetails { get; set; }
    public virtual IEnumerable<Repair>? Repairations { get; set; }
    public virtual IEnumerable<AssetCheck>? AssetChecks { get; set; }
    public virtual IEnumerable<InventoryCheckDetail>? InventoryCheckDetails { get; set; }
}

public enum AssetStatus
{
    [Display(Name = "Hoạt động bình thường")]
    [Color("#1e8239")] // Green color
    Operational = 1,

    [Display(Name = "Không thể sử dụng")]
    [Color("#962612")] // Red color
    Inactive = 2,

    [Display(Name = "Đang bảo dưỡng")]
    [Color("#967508")] // Yellow color
    Maintenance = 3,

    [Display(Name = "Đang sửa chữa")]
    [Color("#d98f1a")] // Orange color
    Repair = 4,

    [Display(Name = "Đang chờ kiểm tra")]
    [Color("#040e91")] // Blue color
    NeedInspection = 5,

    [Display(Name = "Đang chờ thay thế")]
    [Color("#4a0982")] // Purple color
    Replacement = 6,

    [Display(Name = "Đang được điều chuyển")]
    [Color("#1a4e61")] // Teal color
    Transportation = 7,

    [Display(Name = "Hư hại")]
    [Color("#FF6347")] // Tomato color
    Damaged = 8,

    [Display(Name = "Khác")]
    [Color("#4d4d4d")] // Gray color
    Others = 9,
}

public enum RoomRequestStatus{}
public class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");
        
        builder.Property(a => a.TypeId).IsRequired();
        builder.Property(a => a.AssetName).IsRequired();
        builder.Property(a => a.AssetCode).IsRequired(false);
        builder.Property(a => a.StartDateOfUse).IsRequired(false);
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.ImageUrl).IsRequired(false);
        builder.Property(a => a.IsMovable).IsRequired();
        builder.Property(a => a.ManufacturingYear).IsRequired(false);
        builder.Property(a => a.SerialNumber).IsRequired(false);
        builder.Property(a => a.Quantity).IsRequired();
        builder.Property(a => a.LastMaintenanceTime).IsRequired(false);
        builder.Property(a => a.LastCheckedDate).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        builder.Property(a => a.IsRented).IsRequired(false);
        
        // Attributes
        builder.HasIndex(a => a.AssetCode).IsUnique();

        //Relationship
        builder.HasOne(x => x.Type)
            .WithMany(x => x.Assets)
            .HasForeignKey(x => x.TypeId);
        
        // builder.HasOne(x => x.MaintenanceScheduleConfigs)
        //     .WithMany(x => x.Assets)
        //     .HasForeignKey(x => x.MaintenanceConfigId);
        
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

        builder.HasMany(x => x.TransportationDetails)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasMany(x => x.Repairations)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasMany(x => x.AssetChecks)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasMany(x => x.InventoryCheckDetails)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
    }
}   