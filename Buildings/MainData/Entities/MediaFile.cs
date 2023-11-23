using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MediaFile : BaseEntity
{
    public string FileName { get; set; } = null!;
    public string RawUri { get; set; } = null!;
    public string Uri { get; set; } = null!;
    public bool IsVerified { get; set; }
    public FileType FileType { get; set; }
    public string Content { get; set; } = null!;
    public bool IsReported { get; set; }
    public Guid? MaintenanceId { get; set; }
    public Guid? ReplacementId { get; set; }
    public Guid? AssetCheckId { get; set; }
    public Guid? RepairId { get; set; }
    public Guid? TransportationId { get; set; }
    public Guid? InventoryCheckId { get; set; }
    public Guid? ItemId { get; set; }

    // Relationship
    public Maintenance? Maintenance { get; set; }
    public Repair? Repair { get; set; }
    public AssetCheck? AssetCheck { get; set; }
    public Transportation? Transportation { get; set; }
    public Replacement? Replacement { get; set; }
    public InventoryCheck? InventoryCheck { get; set; }

}

public enum FileType
{
    Image = 1,
    Video = 2,
    File = 3
}

public class MediaFileConfig : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("MediaFiles");
        builder.Property(a => a.FileName).IsRequired(false);
        builder.Property(a => a.RawUri).IsRequired(false);
        builder.Property(a => a.Uri).IsRequired();
        builder.Property(a => a.FileType).IsRequired();
        builder.Property(a => a.Content).IsRequired(false);
        builder.Property(a => a.ItemId).IsRequired(false);
        builder.Property(x => x.IsVerified).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.IsReported).IsRequired().HasDefaultValue(false);

        builder.Property(a => a.MaintenanceId).IsRequired(false);
        builder.Property(a => a.ReplacementId).IsRequired(false);
        builder.Property(a => a.RepairId).IsRequired(false);
        builder.Property(a => a.TransportationId).IsRequired(false);
        builder.Property(a => a.AssetCheckId).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.Maintenance)
            .WithMany(x => x.MediaFiles)
            .HasForeignKey(x => x.MaintenanceId);

        builder.HasOne(x => x.AssetCheck)
            .WithMany(x => x.MediaFiles)
            .HasForeignKey(x => x.AssetCheckId);

        builder.HasOne(x => x.Replacement)
            .WithMany(x => x.MediaFiles)
            .HasForeignKey(i => i.ReplacementId);

        builder.HasOne(x => x.Transportation)
            .WithMany(x => x.MediaFiles)
            .HasForeignKey(i => i.TransportationId);

        builder.HasOne(x => x.Repair)
            .WithMany(x => x.MediaFiles)
            .HasForeignKey(i => i.RepairId);

        builder.HasOne(x => x.InventoryCheck)
            .WithMany(x => x.MediaFiles)
            .HasForeignKey(i => i.InventoryCheckId);
    }
}