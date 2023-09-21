﻿using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Transportation : BaseEntity
{
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? Description { get; set; }
    public TransportationStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; } 

    public virtual Asset? Asset { get; set; }
    public virtual Room? ToRoom { get; set; }
    public virtual User? PersonInCharge { get; set; }
    //public virtual User? Creator { get; set; }
}

public enum TransportationStatus
{
    NotStarted = 1,
    InProgress = 1,
    Completed = 3,
    Cancelled = 4,
}

public class TransportationConfig : IEntityTypeConfiguration<Transportation>
{
    public void Configure(EntityTypeBuilder<Transportation> builder)
    {
        builder.ToTable("Transportations");
        builder.Property(x => x.ScheduledDate).IsRequired();
        builder.Property(x => x.ActualDate).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.AssignedTo).IsRequired(false);

        //Relationship

        // builder.HasOne(x => x.Creator)
        //     .WithMany(x => x.Transportations)
        //     .HasForeignKey(x => x.CreatorId);
        
        builder.HasOne(x => x.PersonInCharge)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasOne(x => x.ToRoom)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.ToRoomId);
    }
}