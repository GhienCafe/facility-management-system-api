using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MaintenanceScheduleConfig : BaseEntity
{
    public Guid AssetTypeId { get; set; }
    public TimeUnit TimeUnit { get; set; }
    public int Period { get; set; }
    public DateTime SpecificDate { get; set; }
    
    //
    public virtual AssetType? AssetType { get; set; }
}

public enum TimeUnit
{
    Day = 1,
    Month = 2,
    Year = 3
}

public class MaintenanceScheduleConfigConfig : IEntityTypeConfiguration<MaintenanceScheduleConfig>
{
    public void Configure(EntityTypeBuilder<MaintenanceScheduleConfig> builder)
    {
        builder.ToTable("MaintenanceScheduleConfigs");
        builder.Property(x => x.AssetTypeId).IsRequired();
        builder.Property(x => x.TimeUnit).IsRequired();
        builder.Property(x => x.Period).IsRequired();
        builder.Property(x => x.SpecificDate).IsRequired();
   
        //Relationship
        builder.HasOne(x => x.AssetType)
            .WithMany(x => x.MaintenanceScheduleConfigs)
            .HasForeignKey(x => x.AssetTypeId);

    }
}
