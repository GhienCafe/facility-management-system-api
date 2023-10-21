using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Repairation : BaseRequest
{
    public Guid AssetId { get; set; }
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class RepairationConfig : IEntityTypeConfiguration<Repairation>
{
    public void Configure(EntityTypeBuilder<Repairation> builder)
    {
        builder.ToTable("Repairations");
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(a => a.CategoryId).IsRequired(false);
        builder.Property(a => a.AssetTypeId).IsRequired(false);
        
        // Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Repairations)
            .HasForeignKey(x => x.AssignedTo);
        
        // builder.HasMany(x => x.MediaFiles)
        //     .WithOne(x => x.Repairation)
        //     .HasForeignKey(i => new { i.ItemId });
    }
}