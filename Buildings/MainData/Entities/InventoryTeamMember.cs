using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryTeamMember : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid InventoryTeamId { get; set; }
    public InventoryRole Role { get; set; }
    
    //
    public virtual User? User { get; set; }
    public virtual InventoryTeam? InventoryTeam { get; set; }
}

public enum InventoryRole
{
    TeamLeader = 1,
    PrimaryAuditor = 2,
    Verifier = 3,
    Approver = 4
}

public class InventoryTeamMemberConfig : IEntityTypeConfiguration<InventoryTeamMember>
{
    public void Configure(EntityTypeBuilder<InventoryTeamMember> builder)
    {
        builder.ToTable("InventoryTeamMembers");
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.InventoryTeamId).IsRequired();
        builder.Property(a => a.Role).IsRequired();

        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.InventoryTeamMembers)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        
        builder.HasOne(x => x.InventoryTeam)
            .WithMany(x => x.InventoryTeamMembers)
            .HasForeignKey(x => x.InventoryTeamId);
    }
}   
