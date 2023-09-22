using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class AssetType : BaseEntity
{
    public string TypeCode { get; set; } = null!;
    public string TypeName { get; set; } = null!;
    public string? Description { get; set; }
    public Unit Unit { get; set; }
    public Guid? CategoryId { get; set; }
    
    //
    public virtual IEnumerable<Asset>? Assets { get; set; }
    public virtual Category? Category { get; set; }
    public virtual IEnumerable<MaintenanceScheduleConfig>? MaintenanceScheduleConfigs { get; set; }
}

public enum Unit
{
    Piece = 1, 
    Kg = 2,
    Liter = 3, 
    Meter = 4,
    SquareMeter = 5,
    Hour = 6,
    Bag = 7, 
    Pair = 8,
    Ton = 9, 
    Crate = 10,
    Box = 11
}

public class AssetCategoryConfig : IEntityTypeConfiguration<AssetType>
{
    public void Configure(EntityTypeBuilder<AssetType> builder)
    {
        builder.ToTable("AssetTypes");
        builder.Property(a => a.TypeCode).IsRequired();
        builder.Property(a => a.TypeName).IsRequired();
        builder.Property(a => a.Unit).IsRequired();
        builder.Property(a => a.Description).IsRequired(false);
        
        // Attributes
        builder.HasIndex(a => a.TypeCode).IsUnique();

        //Relationship
        builder.HasMany(x => x.Assets)
            .WithOne(x => x.Type)
            .HasForeignKey(x => x.TypeId);
        
        builder.HasOne(x => x.Category)
            .WithMany(x => x.AssetTypes)
            .HasForeignKey(x => x.CategoryId);
        
        builder.HasMany(x => x.MaintenanceScheduleConfigs)
            .WithOne(x => x.AssetType)
            .HasForeignKey(x => x.AssetTypeId);
    }
}   
