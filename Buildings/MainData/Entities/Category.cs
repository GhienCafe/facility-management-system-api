using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Category : BaseEntity
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? TeamId { get; set; }
    
    //
    public IEnumerable<AssetType>? AssetTypes { get; set; }
    public Team? ResponsibleTeam { get; set; }
}

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.Property(a => a.CategoryName).IsRequired();
        builder.Property(a => a.Description).IsRequired(false);

        //Relationship

        builder.HasMany(x => x.AssetTypes)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(x => x.ResponsibleTeam)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.TeamId);
    }
}   