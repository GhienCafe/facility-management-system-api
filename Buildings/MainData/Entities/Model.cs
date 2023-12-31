﻿using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Model : BaseEntity
{
    public string? ModelName { get; set; }
    public string? ModelCode { get; set; }
    public string? Description { get; set; }
    public int? MaintenancePeriodTime { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? BrandId { get; set; }
    
    //
    public virtual IEnumerable<Asset>? Assets { get; set; }
    public virtual Brand? Brand { get; set; }
}

public class ModelConfig : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.ToTable("Models");

        builder.Property(a => a.ModelName).IsRequired();
        builder.Property(a => a.ModelCode).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        builder.Property(a => a.MaintenancePeriodTime).IsRequired(false);
        builder.Property(a => a.BrandId).IsRequired(false);
        builder.Property(a => a.ImageUrl).IsRequired(false);

        //Relationship
        builder.HasMany(x => x.Assets)
            .WithOne(x => x.Model)
            .HasForeignKey(x => x.ModelId);

        builder.HasOne(x => x.Brand)
            .WithMany(x => x.Models)
            .HasForeignKey(x => x.BrandId);
    }
}   