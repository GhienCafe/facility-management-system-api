using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class AssetType : BaseEntity
{
    public string TypeCode { get; set; } = null!;
    public string TypeName { get; set; } = null!;
    public bool? IsIdentified { get; set; } 
    public string? Description { get; set; }
    public Unit Unit { get; set; }
    public Guid? CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    
    //
    public virtual IEnumerable<Asset>? Assets { get; set; }
    public virtual Category? Category { get; set; }
    //public virtual IEnumerable<MaintenanceScheduleConfig>? MaintenanceScheduleConfigs { get; set; }
}

public enum Unit
{
    [Display(Name = "Cá thể")]
    Individual = 1,

    [Display(Name = "Số lượng")]
    Quantity = 2,

    [Display(Name = "Mét")]
    Meter = 3,

    [Display(Name = "Mét vuông")]
    SquareMeter = 4,

    [Display(Name = "Giờ")]
    Hour = 5,

    [Display(Name = "Bao")]
    Bag = 6,

    [Display(Name = "Đôi")]
    Pair = 7,

    [Display(Name = "Thùng")]
    Crate = 8,

    [Display(Name = "Hộp")]
    Box = 9
}


public class AssetCategoryConfig : IEntityTypeConfiguration<AssetType>
{
    public void Configure(EntityTypeBuilder<AssetType> builder)
    {
        builder.ToTable("AssetTypes");
        builder.Property(a => a.TypeCode).IsRequired();
        builder.Property(a => a.IsIdentified).IsRequired();
        builder.Property(a => a.TypeName).IsRequired();
        builder.Property(a => a.Unit).IsRequired();
        builder.Property(a => a.Description).IsRequired(false);
        builder.Property(a => a.ImageUrl).IsRequired(false);
        
        // Attributes
        builder.HasIndex(a => a.TypeCode).IsUnique();

        //Relationship
        builder.HasMany(x => x.Assets)
            .WithOne(x => x.Type)
            .HasForeignKey(x => x.TypeId);
        
        builder.HasOne(x => x.Category)
            .WithMany(x => x.AssetTypes)
            .HasForeignKey(x => x.CategoryId);
        
        // builder.HasMany(x => x.MaintenanceScheduleConfigs)
        //     .WithOne(x => x.AssetType)
        //     .HasForeignKey(x => x.AssetTypeId);
    }
}   
