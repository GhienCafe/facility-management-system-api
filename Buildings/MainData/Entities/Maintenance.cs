using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MainData.Entities;

public class Maintenance : BaseEntity
{
    public string RequestCode { get; set; } = null!;
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public ActionStatus Status { get; set; }
    public MaintenanceType Type { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }

    //
    public virtual User? PersonInCharge { get; set; }
    public virtual Asset? Asset { get; set; }

}

public enum MaintenanceType
{
    [Display(Name = "Bảo dưỡng định kỳ")]
    Permanent = 1,
    [Display(Name = "Bảo dưỡng đột xuất")]
    Unexpected = 2
}

public class MaintenanceConfig : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.ToTable("Maintenances");
        builder.Property(x => x.RequestCode).IsRequired();
        builder.Property(x => x.RequestedDate).IsRequired();
        builder.Property(x => x.CompletionDate).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Note).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.AssignedTo).IsRequired(false);

        //Relationship

        // builder.HasOne(x => x.Creator)
        //     .WithMany(x => x.Maintenances)
        //     .HasForeignKey(x => x.CreatorId);

        // builder.HasOne(x => x.PersonInCharge)
        //     .WithMany(x => x.Maintenances)
        //     .HasForeignKey(x => x.AssignedTo);

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Maintenances)
            .HasForeignKey(x => x.AssetId);

    }
}
