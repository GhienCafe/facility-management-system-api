using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MaintenanceDetail : BaseEntity
{
    public Guid MaintenanceId { get; set; }
    public string? AssetCode { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public MaintenanceResult? Result { get; set; }
    
    //
    public virtual Maintenance? Maintenance { get; set; }
    public virtual Asset? Asset { get; set; }
}

public enum MaintenanceResult
{
    Normal = 1,
    Completed = 2,
    Repaired = 3,
    RequiresReplacement = 4,
}

public class MaintenanceDetailConfig : IEntityTypeConfiguration<MaintenanceDetail>
{
    public void Configure(EntityTypeBuilder<MaintenanceDetail> builder)
    {
        builder.ToTable("MaintenanceDetails");
        builder.Property(x => x.MaintenanceId).IsRequired();
        builder.Property(x => x.AssetCode).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Note).IsRequired(false);
        builder.Property(x => x.Result).IsRequired(false);
   
        //Relationship
        builder.HasOne(x => x.Maintenance)
            .WithMany(x => x.MaintenanceDetails)
            .HasForeignKey(x => x.MaintenanceId);
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.MaintenanceDetails)
            .HasForeignKey(x => x.AssetCode) 
            .HasPrincipalKey(x => x.AssetCode); 
        
    }
}
