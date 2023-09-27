using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Repairation : BaseEntity
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public ActionStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    
    public Guid? AssetId { get; set; }

    //
    public virtual User? PersonInCharge { get; set; }
    //public virtual User? Creator { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class RepairationConfig : IEntityTypeConfiguration<Repairation>
{
    public void Configure(EntityTypeBuilder<Repairation> builder)
    {
        builder.ToTable("Repairations");

        builder.Property(x => x.RequestedDate).IsRequired();
        builder.Property(x => x.CompletionDate).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Note).IsRequired(false);
        builder.Property(x => x.Reason).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.AssignedTo).IsRequired(false);
        builder.Property(x => x.AssetId).IsRequired();

        //Relationship
        // builder.HasOne(x => x.Creator)
        //     .WithMany(x => x.Repairations)
        //     .HasForeignKey(x => x.CreatorId);
        
        builder.HasOne(x => x.PersonInCharge)
            .WithMany(x => x.Repairations)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Repairations)
            .HasForeignKey(x => x.AssetId);
    }
}