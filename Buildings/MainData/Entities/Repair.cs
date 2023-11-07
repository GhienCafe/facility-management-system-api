using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Repair : BaseRequest
{
    public Guid AssetId { get; set; }
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class RepairConfig : IEntityTypeConfiguration<Repair>
{
    public void Configure(EntityTypeBuilder<Repair> builder)
    {
        builder.ToTable("Repairs");
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(a => a.CategoryId).IsRequired(false);
        builder.Property(a => a.AssetTypeId).IsRequired(false);
        
        // Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Repairs)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasMany(x => x.MediaFiles)
            .WithOne(x => x.Repair)
            .HasForeignKey(i => i.RepairId);
    }
}