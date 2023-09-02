using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Maintenance : BaseEntity
{
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? Description { get; set; }
    public MaintenanceStatus Status { get; set; }
    
    //
    public virtual IEnumerable<MaintenanceParticipant>? MaintenanceParticipants { get; set; }
    public virtual IEnumerable<MaintenanceDetail>? MaintenanceDetails { get; set; }
}

public enum MaintenanceStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
}

public class MaintenanceConfig : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.ToTable("Maintenances");
        builder.Property(x => x.ScheduledDate).IsRequired();
        builder.Property(x => x.ActualDate).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
   
        //Relationship
        builder.HasMany(x => x.MaintenanceParticipants)
            .WithOne(x => x.Maintenance)
            .HasForeignKey(x => x.UserId);
        
        builder.HasMany(x => x.MaintenanceDetails)
            .WithOne(x => x.Maintenance)
            .HasForeignKey(x => x.MaintenanceId);

    }
}
