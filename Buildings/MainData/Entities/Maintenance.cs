﻿using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Maintenance : BaseEntity
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public MaintenanceStatus Status { get; set; }
    public MaintenanceType Type { get; set; }
    public Guid? AssignedTo { get; set; }
    
    //
    public virtual IEnumerable<MaintenanceDetail>? MaintenanceDetails { get; set; }
    public virtual User? User { get; set; }
}

public enum MaintenanceType
{
    Permanent = 1,
    Unexpected  = 2
}

public enum MaintenanceStatus
{
    NotStarted = 1,
    InProgress = 1,
    Completed = 3,
    Cancelled = 4,
}

public class MaintenanceConfig : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.ToTable("Maintenances");
        builder.Property(x => x.RequestedDate).IsRequired();
        builder.Property(x => x.CompletionDate).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.AssignedTo).IsRequired(false);
   
        //Relationship
        builder.HasMany(x => x.MaintenanceDetails)
            .WithOne(x => x.Maintenance)
            .HasForeignKey(x => x.MaintenanceId);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Maintenances)
            .HasForeignKey(x => x.CreatorId);

    }
}
