﻿// using System.ComponentModel.DataAnnotations;
// using AppCore.Data;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace MainData.Entities;
//
// public class MaintenanceScheduleConfig : BaseEntity
// {
//     public string Code { get; set; } = null!;
//     public int RepeatIntervalInMonths { get; set; } 
//     public string? Description { get; set; }
//     
//     //
//     public virtual IEnumerable<Asset>? Assets { get; set; }
// }
//
// public class MaintenanceScheduleConfigConfig : IEntityTypeConfiguration<MaintenanceScheduleConfig>
// {
//     public void Configure(EntityTypeBuilder<MaintenanceScheduleConfig> builder)
//     {
//         builder.ToTable("MaintenanceScheduleConfigs");
//         builder.Property(x => x.Code).IsRequired();
//         builder.Property(x => x.RepeatIntervalInMonths).IsRequired();
//         builder.Property(x => x.Description).IsRequired(false);
//
//        //builder.HasIndex(x => x.Code).IsUnique();
//    
//         //Relationship
//         builder.HasMany(x => x.Assets)
//             .WithOne(x => x.MaintenanceScheduleConfigs)
//             .HasForeignKey(x => x.MaintenanceConfigId);
//
//     }
// }
