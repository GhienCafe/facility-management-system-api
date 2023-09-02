using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class HandoverParticipant : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid AssetHandoverId { get; set; }
    public AssetHandoverRole Role { get; set; }
    
    //
    public virtual AssetHandover? AssetHandover { get; set; }
    public virtual User? User { get; set; }
}

public enum AssetHandoverRole
{
    HandoverProvider = 1,
    HandoverReceiver = 2,
    Verifier = 3,
    Approver = 4
}

public class HandoverParticipantConfig : IEntityTypeConfiguration<HandoverParticipant>
{
    public void Configure(EntityTypeBuilder<HandoverParticipant> builder)
    {
        builder.ToTable("HandoverParticipants");
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.AssetHandoverId).IsRequired();
        builder.Property(x => x.Role).IsRequired();
   
        //Relationship
        builder.HasOne(x => x.AssetHandover)
            .WithMany(x => x.HandoverParticipants)
            .HasForeignKey(x => x.AssetHandoverId);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.HandoverParticipants)
            .HasForeignKey(x => x.UserId);

    }
}
