using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MaintenanceScheduleConfig : BaseEntity
{
    public Guid AssetId { get; set; }
    public TimeUnit TimeUnit { get; set; }
    public int Period { get; set; }
    public DateTime SpecificDate { get; set; }
    public Guid? AssignedTo { get; set; }
    
    //
    public virtual Asset? Asset { get; set; }
    public virtual User? PersonInCharge { get; set; }
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
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.TimeUnit).IsRequired();
        builder.Property(x => x.Period).IsRequired();
        builder.Property(x => x.SpecificDate).IsRequired();
   
        //Relationship
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.MaintenanceScheduleConfigs)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasOne(x => x.PersonInCharge)
            .WithMany(x => x.MaintenanceScheduleConfigs)
            .HasForeignKey(x => x.AssignedTo);

    }
}
