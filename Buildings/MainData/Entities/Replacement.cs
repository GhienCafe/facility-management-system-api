using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Replacement : BaseEntity
{
    public Guid AssetId { get; set; }
    public Guid RequestId { get; set; }
    public Guid? NewAssetId { get; set; }
    
    //
    public virtual Asset? Asset { get; set; }
    public virtual Request? Request { get; set; }
    //public virtual Asset? NewAsset { get; set; }
}

public class ReplacementConfig : IEntityTypeConfiguration<Replacement>
{
    public void Configure(EntityTypeBuilder<Replacement> builder)
    {
        builder.ToTable("Replacements");
        builder.Property(x => x.NewAssetId).IsRequired(false);
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.RequestId).IsRequired();
        
        //Relationship
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Replacements)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasOne(x => x.Request)
            .WithMany(x => x.Replacements)
            .HasForeignKey(x => x.RequestId);

       //builder.Ignore(x => x.NewAsset);

    }
}
