using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Replacement : BaseEntity
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Reason { get; set; }
    public ReplacementStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    
    //
    public virtual IEnumerable<ReplacementDetail>? ReplacementDetails { get; set; }
    public virtual User? User { get; set; }
}

public enum ReplacementStatus
{
    NotStarted = 1,
    InProgress = 1,
    Completed = 3,
    Cancelled = 4,
}

public class ReplacementConfig : IEntityTypeConfiguration<Replacement>
{
    public void Configure(EntityTypeBuilder<Replacement> builder)
    {
        builder.ToTable("Replacements");
        builder.Property(x => x.RequestedDate).IsRequired();
        builder.Property(x => x.CompletionDate).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Reason).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.AssignedTo).IsRequired(false);
   
        //Relationship
        builder.HasMany(x => x.ReplacementDetails)
            .WithOne(x => x.Replacement)
            .HasForeignKey(x => x.ReplacementId);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Replacements)
            .HasForeignKey(x => x.CreatorId);

    }
}
