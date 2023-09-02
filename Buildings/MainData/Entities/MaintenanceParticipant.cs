using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MaintenanceParticipant : BaseEntity
{
    public Guid MaintenanceId { get; set; }
    public Guid UserId { get; set; }
    public MaintenanceRole Role { get; set; }
    
    //
    public virtual Maintenance? Maintenance { get; set; }
    public virtual User? User { get; set; }
}

public enum MaintenanceRole
{
    Requester = 1,
    MaintenancePerformer = 2,
    Supervisor = 3,
    Approver = 4,
    Verifier =5,
}

public class MaintenanceParticipantConfig : IEntityTypeConfiguration<MaintenanceParticipant>
{
    public void Configure(EntityTypeBuilder<MaintenanceParticipant> builder)
    {
        builder.ToTable("MaintenanceParticipants");
        builder.Property(x => x.MaintenanceId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Role).IsRequired();
   
        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.MaintenanceParticipants)
            .HasForeignKey(x => x.UserId);
        
        builder.HasOne(x => x.Maintenance)
            .WithMany(x => x.MaintenanceParticipants)
            .HasForeignKey(x => x.MaintenanceId);

    }
}


