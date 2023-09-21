using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Replacement : BaseEntity
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public ReplacementStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? NewAssetId { get; set; }
    
    //
    public virtual User? PersonInCharge { get; set; }
    //public virtual User? Creator { get; set; }
    public virtual Asset? Asset { get; set; }
    public virtual Asset? NewAsset { get; set; }
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
        builder.Property(x => x.NewAssetId).IsRequired(false);
        builder.Property(x => x.AssetId).IsRequired(false);
        
        //Relationship

        // builder.HasOne(x => x.Creator)
        //     .WithMany(x => x.Replacements)
        //     .HasForeignKey(x => x.CreatorId);
        
        builder.HasOne(x => x.PersonInCharge)
            .WithMany(x => x.Replacements)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Replacements)
            .HasForeignKey(x => x.AssetId);
        
        // builder.HasOne(x => x.NewAsset)
        //     .WithMany(x => x.Replacements)
        //     .HasForeignKey(x => x.AssetId);

        builder.Ignore(x => x.NewAsset);

    }
}
