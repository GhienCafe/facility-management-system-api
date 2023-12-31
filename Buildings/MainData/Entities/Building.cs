﻿using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Building : BaseEntity
{
    public string? BuildingName { get; set; }
    public string? BuildingCode { get; set; }
    public Guid? CampusId { get; set; }
    public string? Description { get; set; }
    public double? TotalArea { get; set; }
    //Relationship
    public virtual IEnumerable<Floor>? Floors { get; set; }
    public virtual Campus? Campus { get; set; }
}

public class BuildingConfig : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings");
        builder.Property(a => a.BuildingName).IsRequired(false);
        builder.Property(a => a.BuildingCode).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        builder.Property(a => a.TotalArea).IsRequired(false);
        builder.Property(a => a.CampusId).IsRequired();

        builder.HasOne(x => x.Campus)
            .WithMany(x => x.Buildings)
            .HasForeignKey(x => x.CampusId);
        
        builder.HasMany(x => x.Floors)
            .WithOne(x => x.Buildings)
            .HasForeignKey(x => x.BuildingId);
    }
}