using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class AssetHandover : BaseEntity
{
    public string HandOverCode { get; set; } = null!;
    public string? Note { get; set; }
    public AssetHandoversStatus Status { get; set; }
    
    //
    public virtual IEnumerable<HandoverParticipant>? HandoverParticipants { get; set; }
    public virtual IEnumerable<HandoverDetail>? HandoverDetails { get; set; }

}

public enum AssetHandoversStatus
{
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
    PendingApproval = 4,
    Rejected = 5,
    Canceled = 6,
    Verified = 7,
    Closed = 8,
    PendingAdditions = 9,
    MarkedAsFaulty = 10,
    Unclassified = 11
}

public class AssetHandoverConfig : IEntityTypeConfiguration<AssetHandover>
{
    public void Configure(EntityTypeBuilder<AssetHandover> builder)
    {
        builder.ToTable("AssetHandovers");
        builder.Property(x => x.HandOverCode).IsRequired();
        builder.Property(x => x.Note).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
        
        //Attribute
        builder.HasIndex(x => x.HandOverCode).IsUnique();
   
        //Relationship
        builder.HasMany(x => x.HandoverParticipants)
            .WithOne(x => x.AssetHandover)
            .HasForeignKey(x => x.AssetHandoverId);
        
        builder.HasMany(x => x.HandoverDetails)
            .WithOne(x => x.AssetHandover)
            .HasForeignKey(x => x.AssetHandoverId);

    }
}
