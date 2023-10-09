using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class TeamMember : BaseEntity
{
    public Guid MemberId { get; set; }
    public Guid TeamId { get; set; }
    public bool IsLead { get; set; }
    
    //
    public virtual Team? Team { get; set; }
    public virtual User? Member { get; set; }
}

public class TeamMemberConfig : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");

        builder.Property(a => a.MemberId).IsRequired();
        builder.Property(a => a.TeamId).IsRequired();
        builder.Property(a => a.IsLead).IsRequired().HasDefaultValue(false);

        //Relationship

        builder.HasOne(x => x.Team)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.TeamId);
        
        builder.HasOne(x => x.Team)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.TeamId);
    }
}   