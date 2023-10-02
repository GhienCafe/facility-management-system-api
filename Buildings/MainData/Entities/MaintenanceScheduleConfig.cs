﻿using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MaintenanceScheduleConfig : BaseEntity
{
    public Guid AssetId { get; set; }
    public int Period { get; set; } //
    
    //
    public virtual Asset? Asset { get; set; }
}

public class MaintenanceScheduleConfigConfig : IEntityTypeConfiguration<MaintenanceScheduleConfig>
{
    public void Configure(EntityTypeBuilder<MaintenanceScheduleConfig> builder)
    {
        builder.ToTable("MaintenanceScheduleConfigs");
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.Period).IsRequired();
   
        //Relationship
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.MaintenanceScheduleConfigs)
            .HasForeignKey(x => x.AssetId);

    }
}
