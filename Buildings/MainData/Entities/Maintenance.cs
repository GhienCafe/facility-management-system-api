using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MainData.Entities;

public class Maintenance : BaseEntity
{
    public Guid? AssetId { get; set; }
    public Guid? RequestId { get; set; }
    public string? Notes { get; set; } //Result
    
    public virtual Asset? Asset { get; set; }
    public virtual Request? Request { get; set; }
}

public class MaintenanceConfig : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.ToTable("Maintenances");
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.RequestId).IsRequired();
        builder.Property(x => x.Notes).IsRequired(false);

        //Relationship

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Maintenances)
            .HasForeignKey(x => x.AssetId);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Maintenances)
            .HasForeignKey(x => x.RequestId);
    }
}
