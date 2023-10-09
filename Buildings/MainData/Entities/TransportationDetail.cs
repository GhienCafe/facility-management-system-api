using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class TransportationDetail : BaseEntity
{
    public Guid AssetId { get; set; }
    public Guid TransportationId { get; set; }
    public DateTime? ReuqestDate { get; set; }
    public int? Quantity { get; set; }

    //
    public virtual Asset? Asset { get; set; }
    public virtual Transportation? Transportation { get; set; }
}

public class TransportationDetailConfig : IEntityTypeConfiguration<TransportationDetail>
{
    public void Configure(EntityTypeBuilder<TransportationDetail> builder)
    {
        builder.ToTable("TransportationDetails");
        builder.Property(x => x.AssetId).IsRequired(false);
        builder.Property(x => x.TransportationId).IsRequired(false);
        builder.Property(x => x.ReuqestDate).IsRequired(false);
        builder.Property(x => x.Quantity).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.TransportationDetails)
            .HasForeignKey(x => x.AssetId);

        builder.HasOne(x => x.Transportation)
            .WithMany(x => x.TransportationDetails)
            .HasForeignKey(x => x.AssetId);
    }
}


