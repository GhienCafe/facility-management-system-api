using AppCore.Data;
using AppCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Team : BaseEntity
{
    public string TeamName { get; set; } = null!;
    public string? Description { get; set; }
    
    //
    public IEnumerable<User>? Users { get; set; }
    public IEnumerable<Category>? Categories { get; set; }
}

public class TeamConfig : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.Property(a => a.TeamName).IsRequired();
        builder.Property(a => a.Description).IsRequired(false);

        //Relationship

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Team)
            .HasForeignKey(x => x.TeamId);

        builder.HasMany(x => x.Categories)
            .WithOne(x => x.ResponsibleTeam)
            .HasForeignKey(x => x.TeamId);
    }
}   