using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryDetailConfig : BaseEntity
{
    public Guid InventoryConfigId { get; set; }
    public DateTime InventoryDate { get; set; }
    
    //Relationship
    public InventoryCheckConfig? InventoryCheckConfig { get; set; }
}

public class InventoryDetailConfigConfig : IEntityTypeConfiguration<InventoryDetailConfig>
{
    public void Configure(EntityTypeBuilder<InventoryDetailConfig> builder)
    {
        builder.ToTable("InventoryDetailConfigs");
        builder.Property(x => x.InventoryConfigId).IsRequired();
        builder.Property(x => x.InventoryDate).IsRequired();

        builder.HasOne(x => x.InventoryCheckConfig)
            .WithMany(x => x.InventoryDetailConfigs)
            .HasForeignKey(x => x.InventoryConfigId);
    }
}
