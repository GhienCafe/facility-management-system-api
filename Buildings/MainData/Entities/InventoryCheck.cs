using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryCheck : BaseRequest
{
    //Relationship
    public Guid InventoryCheckConfigId { get; set; }
    public Guid RoomId { get; set; }

    public virtual Room? Room { get; set; }
    public InventoryCheckConfig? InventoryCheckConfig { get; set; }
    public IEnumerable<InventoryCheckDetail>? InventoryCheckDetails { get; set; }
}

public class InventoryCheckDbConfig : IEntityTypeConfiguration<InventoryCheck>
{
    public void Configure(EntityTypeBuilder<InventoryCheck> builder)
    {
        builder.ToTable("InventoryChecks");
        builder.Property(x => x.InventoryCheckConfigId).IsRequired();
        builder.Property(x => x.RoomId).IsRequired();

        //Relationship

        builder.HasMany(x => x.InventoryCheckDetails)
            .WithOne(x => x.InventoryCheck)
            .HasForeignKey(i => i.InventoryCheckId);
        
        builder.HasOne(x => x.InventoryCheckConfig)
            .WithMany(x => x.InventoryChecks)
            .HasForeignKey(i => i.InventoryCheckConfigId);
        
        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.InventoryChecks)
            .HasForeignKey(x => x.AssignedTo);

        builder.HasMany(x => x.MediaFiles)
            .WithOne(x => x.InventoryCheck)
            .HasForeignKey(i => i.MaintenanceId);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.InventoryChecks)
            .HasForeignKey(x => x.RoomId);
    }
}
