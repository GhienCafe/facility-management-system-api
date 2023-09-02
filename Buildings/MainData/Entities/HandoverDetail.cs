using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class HandoverDetail : BaseEntity
{
    public Guid AssetHandoverId { get; set; }
    public string AssetCode { get; set; } = null!;
    public double Quantity { get; set; }
    
    //
    public virtual AssetHandover? AssetHandover { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class HandoverDetailConfig : IEntityTypeConfiguration<HandoverDetail>
{
    public void Configure(EntityTypeBuilder<HandoverDetail> builder)
    {
        builder.ToTable("HandoverDetails");
        builder.Property(x => x.AssetHandoverId).IsRequired();
        builder.Property(x => x.AssetCode).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
   
        //Relationship
        builder.HasOne(x => x.AssetHandover)
            .WithMany(x => x.HandoverDetails)
            .HasForeignKey(x => x.AssetHandoverId);
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.HandoverDetails)
            .HasForeignKey(x => x.AssetCode)
            .HasPrincipalKey(x => x.AssetCode);


    }
}