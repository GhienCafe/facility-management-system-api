using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MaintenanceDetail : BaseEntity
{
    public Guid MaintenanceId { get; set; }
    public Guid? AssetId { get; set; }
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    
    //
    public virtual Maintenance? Maintenance { get; set; }
    public virtual Asset? Asset { get; set; }
}

// public enum MaintenanceResult
// {
//     Normal = 1,
//     Completed = 2,
//     Repaired = 3,
//     RequiresReplacement = 4,
// }

public class MaintenanceDetailConfig : IEntityTypeConfiguration<MaintenanceDetail>
{
    public void Configure(EntityTypeBuilder<MaintenanceDetail> builder)
    {
        builder.ToTable("MaintenanceDetails");
        builder.Property(x => x.MaintenanceId).IsRequired();
        builder.Property(x => x.AssetId).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.IsDone).IsRequired();
   
        //Relationship
        builder.HasOne(x => x.Maintenance)
            .WithMany(x => x.MaintenanceDetails)
            .HasForeignKey(x => x.MaintenanceId);

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.MaintenanceDetails)
            .HasForeignKey(x => x.AssetId);

    }
}
