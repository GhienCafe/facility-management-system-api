using System.ComponentModel.DataAnnotations;
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

    //
    public virtual AssetType? Type { get; set; }
    public virtual IEnumerable<MaintenanceScheduleConfig>? MaintenanceScheduleConfigs { get; set; }
    public virtual Model? Model { get; set; }
    public virtual IEnumerable<RoomAsset>? RoomAssets { get; set; }
    public virtual IEnumerable<Maintenance>? Maintenances { get; set; }
    public virtual IEnumerable<Replacement>? Replacements { get; set; }
    public virtual IEnumerable<Transportation>? Transportations { get; set; }
    public virtual IEnumerable<Repairation>? Repairations { get; set; }
    public virtual IEnumerable<AssetCheck>? AssetChecks { get; set; }
}

public enum AssetStatus
{
    [Display(Name = "Hoạt động")]
    Operational = 1,

    [Display(Name = "Ngưng hoạt động")]
    Inactive = 2,

    [Display(Name = "Bảo dưỡng")]
    Maintenance = 3,

    [Display(Name = "Sửa chữa")]
    Repair = 4,

    [Display(Name = "Thanh lý")]
    Disposed = 5,

    [Display(Name = "Hỏng")]
    OutOfOrder = 6,

    [Display(Name = "Chờ duyệt")]
    Pending = 7,

    [Display(Name = "Không khả dụng")]
    NotAvailable = 8,

    [Display(Name = "Cần kiểm tra")]
    NeedInspection = 9,

    [Display(Name = "Nâng cấp")]
    Upgraded = 10,

    [Display(Name = "Thay thế")]
    Replacement = 11
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
        
        builder.HasMany(x => x.MaintenanceScheduleConfigs)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
        
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
        
        builder.HasMany(x => x.AssetChecks)
            .WithOne(x => x.Asset)
            .HasForeignKey(x => x.AssetId);
    }
}   