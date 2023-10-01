using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Repairation : BaseEntity
{
    public Guid? AssetId { get; set; }
    public Guid? RequestId { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }

    //
    public virtual Asset? Asset { get; set; }
    public virtual Request? Request { get; set; }
}

public class RepairationConfig : IEntityTypeConfiguration<Repairation>
{
    public void Configure(EntityTypeBuilder<Repairation> builder)
    {
        builder.ToTable("Repairations");

        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Notes).IsRequired(false);
        builder.Property(x => x.RequestId).IsRequired();
        builder.Property(x => x.AssetId).IsRequired();

        //Relationship
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Repairations)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasOne(x => x.Request)
            .WithMany(x => x.Repairations)
            .HasForeignKey(x => x.RequestId);
    }
}