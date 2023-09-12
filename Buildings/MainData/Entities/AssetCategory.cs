using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class AssetCategory : BaseEntity
{
    public string CategoryCode { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public Unit Unit { get; set; }
    
    //
    public virtual IEnumerable<Asset>? Assets { get; set; }
    public virtual IEnumerable<RequestDetail>? RequestDetails { get; set; }
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

public class AssetCategoryConfig : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("AssetCategories");
        builder.Property(a => a.CategoryCode).IsRequired();
        builder.Property(a => a.CategoryName).IsRequired();
        builder.Property(a => a.Unit).IsRequired();
        builder.Property(a => a.Description).IsRequired(false);
        
        // Attributes
        builder.HasIndex(a => a.CategoryCode).IsUnique();

        //Relationship
        builder.HasMany(x => x.Assets)
            .WithOne(x => x.AssetCategory)
            .HasForeignKey(x => x.CategoryId);
        
        builder.HasMany(x => x.RequestDetails)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId);
    }
}   
