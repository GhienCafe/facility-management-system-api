using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Transportation : BaseEntity
{
    public int? Quantity { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? RequestId { get; set; }
    public Guid? ToRoomId { get; set; } // For internal

    public virtual Asset? Asset { get; set; }
    public virtual Request? Request { get; set; }
    public virtual Room? ToRoom { get; set; }
}

public class TransportationConfig : IEntityTypeConfiguration<Transportation>
{
    public void Configure(EntityTypeBuilder<Transportation> builder)
    {
        builder.ToTable("Transportations");
        builder.Property(x => x.Quantity).IsRequired(false);
        builder.Property(x => x.RequestId).IsRequired();
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.ToRoomId).IsRequired(false);

        //Relationship

        // builder.HasOne(x => x.Creator)
        //     .WithMany(x => x.Transportations)
        //     .HasForeignKey(x => x.CreatorId);

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.AssetId);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.RequestId);
        
        builder.HasOne(x => x.ToRoom)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.ToRoomId);
    }
}