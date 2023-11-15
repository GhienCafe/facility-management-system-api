using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryCheck : BaseRequest
{
    //Relationship

    public IEnumerable<InventoryCheckDetail>? InventoryCheckDetails { get; set; }
}

public class InventoryCheckDbConfig : IEntityTypeConfiguration<InventoryCheck>
{
    public void Configure(EntityTypeBuilder<InventoryCheck> builder)
    {
        builder.ToTable("InventoryChecks");

        //Relationship

        builder.HasMany(x => x.InventoryCheckDetails)
            .WithOne(x => x.InventoryCheck)
            .HasForeignKey(i => i.InventoryCheckId);
        
        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.InventoryChecks)
            .HasForeignKey(x => x.AssignedTo);

        builder.HasMany(x => x.MediaFiles)
            .WithOne(x => x.InventoryCheck)
            .HasForeignKey(i => i.InventoryCheckId);
    }
}
