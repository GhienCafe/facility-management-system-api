using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryCheckConfig : BaseEntity
{
    public int NotificationDays  { get; set; }
    public string? Content { get; set; }
    public bool IsActive { get; set; }
    
    //Relationship
    public IEnumerable<InventoryDetailConfig>? InventoryDetailConfigs { get; set; }
}

public class InventoryCheckConfigConfig : IEntityTypeConfiguration<InventoryCheckConfig>
{
    public void Configure(EntityTypeBuilder<InventoryCheckConfig> builder)
    {
        builder.ToTable("InventoryCheckConfig");
        builder.Property(x => x.NotificationDays).IsRequired();
        builder.Property(x => x.Content).IsRequired(false);
        builder.Property(x => x.IsActive).HasDefaultValue(false);

        builder.HasMany(x => x.InventoryDetailConfigs)
            .WithOne(x => x.InventoryCheckConfig)
            .HasForeignKey(x => x.InventoryConfigId);
    }
}
