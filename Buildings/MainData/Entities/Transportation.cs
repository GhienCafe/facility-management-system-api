using AppCore.Data;
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

    public virtual IEnumerable<TransportationDetail>? TransportationDetails { get; set; }
    public virtual User? User { get; set; }
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
        builder.HasMany(x => x.TransportationDetails)
            .WithOne(x => x.Transportation)
            .HasForeignKey(x => x.TransportationId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.CreatorId);
    }
}