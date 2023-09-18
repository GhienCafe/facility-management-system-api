using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;
public class TransportationDetail : BaseEntity
{
    public Guid TransportationId { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? SourceLocation { get; set; }
    public Guid? DestinationLocation { get; set; }
    public bool IsDone { get; set; }
    public string? Description { get; set; }

    public virtual Transportation? Transportation { get; set; }
    public virtual Asset? Asset { get; set; }
}

public class TransportationDetailConfig : IEntityTypeConfiguration<TransportationDetail>
{
    public void Configure(EntityTypeBuilder<TransportationDetail> builder)
    {
        builder.ToTable("TransportationDetails");
        builder.Property(x => x.TransportationId).IsRequired();
        builder.Property(x => x.AssetId).IsRequired(false);
        builder.Property(x => x.SourceLocation).IsRequired(false);
        builder.Property(x => x.DestinationLocation).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.IsDone).IsRequired();

        //Relationship
        builder.HasOne(x => x.Transportation)
            .WithMany(x => x.TransportationDetails)
            .HasForeignKey(x => x.TransportationId);

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.TransportationDetails)
            .HasForeignKey(x => x.AssetId);
    }
}


