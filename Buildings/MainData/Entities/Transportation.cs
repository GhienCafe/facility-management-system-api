using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Transportation : BaseRequest
{
    public int? Quantity { get; set; }
    public Guid? ToRoomId { get; set; } // For internal
    
    public virtual Room? ToRoom { get; set; }
    public virtual ICollection<TransportationDetail>? TransportationDetails { get; set; }
}

public class TransportationConfig : IEntityTypeConfiguration<Transportation>
{
    public void Configure(EntityTypeBuilder<Transportation> builder)
    {
        builder.ToTable("Transportations");
        builder.Property(x => x.Quantity).IsRequired(false);
        //builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.ToRoomId).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.ToRoom)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.ToRoomId);

        builder.HasMany(x => x.TransportationDetails)
            .WithOne(x => x.Transportation)
            .HasForeignKey(x => x.TransportationId);
    }
}