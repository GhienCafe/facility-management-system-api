using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Replacement : BaseRequest
{
    public Guid AssetId { get; set; }
    public Guid RoomId { get; set; }
    public Guid NewAssetId { get; set; }
    public Guid NewRoomId { get; set; }
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class ReplacementConfig : IEntityTypeConfiguration<Replacement>
{
    public void Configure(EntityTypeBuilder<Replacement> builder)
    {
        builder.ToTable("Replacements");
        builder.Property(x => x.NewAssetId).IsRequired();
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.NewRoomId).IsRequired();
        builder.Property(x => x.RoomId).IsRequired();
        builder.Property(a => a.CategoryId).IsRequired(false);
        builder.Property(a => a.AssetTypeId).IsRequired(false);


        // Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Replacements)
            .HasForeignKey(x => x.AssignedTo);

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Replacements)
            .HasForeignKey(x => x.AssetId);

        builder.HasMany(x => x.MediaFiles)
            .WithOne(x => x.Replacement)
            .HasForeignKey(i => i.RepairId);

    }
}
