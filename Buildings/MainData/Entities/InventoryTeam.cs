using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryTeam : BaseEntity
{
    public Guid InventoryId { get; set; }
    public string? TeamName { get; set; }
    public DateTime? InventoryDate { get; set; }
    
    //
    public virtual Inventory? Inventory { get; set; }
   public virtual IEnumerable<InventoryTeamMember>? InventoryTeamMembers { get; set; }
}

public class InventoryTeamConfig : IEntityTypeConfiguration<InventoryTeam>
{
    public void Configure(EntityTypeBuilder<InventoryTeam> builder)
    {
        builder.ToTable("InventoryTeams");
        builder.Property(a => a.InventoryId).IsRequired();
        builder.Property(a => a.TeamName).IsRequired(false);
        builder.Property(a => a.InventoryDate).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.Inventory)
            .WithMany(x => x.InventoryTeams)
            .HasForeignKey(x => x.InventoryId);
        
        builder.HasMany(x => x.InventoryTeamMembers)
            .WithOne(x => x.InventoryTeam)
            .HasForeignKey(x => x.InventoryTeamId).OnDelete(DeleteBehavior.NoAction);
    }
}   
