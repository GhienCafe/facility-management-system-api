using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class TransportationDetail : BaseEntity
{
    public Guid? AssetId { get; set; }
    public Guid? TransportationId { get; set; }
    public DateTime? RequestDate { get; set; }
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
        builder.Property(a => a.AssetId).IsRequired();
        builder.Property(a => a.TransportationId).IsRequired();
        builder.Property(a => a.RequestDate).IsRequired();
        builder.Property(a => a.Quantity).IsRequired();

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.TransportationDetails)
            .HasForeignKey(x => x.AssetId);

        builder.HasOne(x => x.Transportation)
            .WithMany(x => x.TransportationDetails)
            .HasForeignKey(x => x.TransportationId);
    }
}

