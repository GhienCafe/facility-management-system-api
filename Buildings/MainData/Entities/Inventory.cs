using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Inventory : BaseEntity
{
    public string? InventoryCode { get; set; }
    public Guid CampusId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? Content { get; set; }
    public InventoryStatus Status { get; set; }
    public InventoryType Type { get; set; }
    
    //
    public virtual Campus? Campus { get; set; }
    public virtual IEnumerable<InventoryTeam>? InventoryTeams { get; set; }
    public virtual IEnumerable<InventoryDetail>? InventoryDetails { get; set; }
}

public enum InventoryStatus
{
    NotAudited = 1,
    InProgress = 2,
    Audited = 3,
}

public enum InventoryType
{
    Periodic = 1,
    Unexpected = 2,
}

public class InventoryConfig : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");
        builder.Property(a => a.InventoryCode).IsRequired();
        builder.Property(a => a.CampusId).IsRequired();
        builder.Property(a => a.ScheduledDate).IsRequired();
        builder.Property(a => a.Content).IsRequired(false);
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.Type).IsRequired();
        
        //Attribute
        builder.HasIndex(x => x.InventoryCode).IsUnique();

        //Relationship
        builder.HasOne(x => x.Campus)
            .WithMany(x => x.Inventories)
            .HasForeignKey(x => x.CampusId);
        
        builder.HasMany(x => x.InventoryTeams)
            .WithOne(x => x.Inventory)
            .HasForeignKey(x => x.InventoryId);
        
        builder.HasMany(x => x.InventoryDetails)
            .WithOne(x => x.Inventory)
            .HasForeignKey(x => x.InventoryId);
    }
}   