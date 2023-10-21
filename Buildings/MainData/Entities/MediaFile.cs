﻿using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MediaFile : BaseEntity
{
    public string FileName { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string RawUri { get; set; } = null!;
    public string Uri { get; set; } = null!;
    public string Extensions { get; set; } = null!;
    public FileType FileType { get; set; }
    public string Content { get; set; } = null!;
    
    public Guid? MaintenanceId { get; set; }
    public Guid? ReplacementId { get; set; }
    public Guid? AssetCheckId { get; set; }
    public Guid? RepairationId { get; set; }
    public Guid? TransportationId { get; set; }
    public Guid? ItemId { get; set; }
    
    // Relationship
    public Maintenance? Maintenance { get; set; }
    public Repairation? Repairation { get; set; }
    public AssetCheck? AssetCheck { get; set; }
    public Transportation? Transportation { get; set; }
    public Replacement? Replacement { get; set; }
    
}

public enum FileType
{
    Image = 1,
    Video = 2,
    File = 3
}

public class MediaFileConfig : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("MediaFiles");
        builder.Property(a => a.FileName).IsRequired(false);
        builder.Property(a => a.Key).IsRequired(false);
        builder.Property(a => a.RawUri).IsRequired(false);
        builder.Property(a => a.Uri).IsRequired();
        builder.Property(a => a.Extensions).IsRequired(false);
        builder.Property(a => a.FileType).IsRequired();
        builder.Property(a => a.Content).IsRequired(false); 
        builder.Property(a => a.ItemId).IsRequired(false);
        
        // builder.Property(a => a.MaintenanceId).IsRequired(false);
        // builder.Property(a => a.ReplacementId).IsRequired(false);
        // builder.Property(a => a.RepairationId).IsRequired(false);
        // builder.Property(a => a.TransportationId).IsRequired(false);
        // builder.Property(a => a.AssetCheckId).IsRequired(false);
        
        //Relationship
        // builder.HasOne(x => x.Maintenance)
        //     .WithMany(x => x.MediaFiles)
        //     .HasForeignKey(i => new { i.ItemId });
        //
        // builder.HasOne(x => x.AssetCheck)
        //     .WithMany(x => x.MediaFiles)
        //     .HasForeignKey(x => new {x.ItemId})
        //     .IsRequired(false);
        //
        // builder.HasOne(x => x.Replacement)
        //     .WithMany(x => x.MediaFiles)
        //     .HasForeignKey(i => new { i.ItemId });
        //
        // builder.HasOne(x => x.Transportation)
        //     .WithMany(x => x.MediaFiles)
        //     .HasForeignKey(i => new { i.ItemId });
        //
        // builder.HasOne(x => x.Repairation)
        //     .WithMany(x => x.MediaFiles)
        //     .HasForeignKey(i => new { i.ItemId });
    }
}