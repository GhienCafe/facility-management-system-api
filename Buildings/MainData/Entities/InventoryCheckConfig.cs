using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class InventoryCheckConfig : BaseEntity
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public DateTime? CheckDate { get; set; }
    public DateTime? LastCheckedDate { get; set; }
    
    //Relationship
    public IEnumerable<InventoryCheck>? InventoryChecks { get; set; }
}

public class InventoryCheckConfigConfig : IEntityTypeConfiguration<InventoryCheckConfig>
{
    public void Configure(EntityTypeBuilder<InventoryCheckConfig> builder)
    {
        builder.ToTable("InventoryCheckConfigs");
        builder.Property(x => x.CheckPeriod).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.LastCheckedDate).HasColumnType("datetime").IsRequired(false);
        builder.Property(x => x.CheckDate).HasColumnType("datetime").IsRequired(false);
        builder.Property(x => x.Status).IsRequired().HasDefaultValue(false);

        //Relationship
    }
}
