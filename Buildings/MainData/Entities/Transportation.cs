using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Transportation : BaseEntity
{
    //public Guid AssetId { get; set; }
    public string RequestCode { get; set; } = null!;
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public bool IsInternal { get; set; }
    public int? Quantity { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? ToRoomId { get; set; } // For internal
    
    public virtual Room? ToRoom { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<TransportationDetail>? TransportationDetails { get; set; }
}

public class TransportationConfig : IEntityTypeConfiguration<Transportation>
{
    public void Configure(EntityTypeBuilder<Transportation> builder)
    {
        builder.ToTable("Transportations");
        builder.Property(a => a.RequestCode).IsRequired();
        builder.Property(a => a.RequestDate).IsRequired(false);
        builder.Property(a => a.CompletionDate).IsRequired(false);
        builder.Property(a => a.Status).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        builder.Property(a => a.Notes).IsRequired(false);
        builder.Property(a => a.IsInternal).IsRequired();
        builder.Property(x => x.Quantity).IsRequired(false);
        //builder.Property(x => x.AssetId).IsRequired();
        builder.Property(a => a.AssignedTo).IsRequired(false);
        builder.Property(x => x.ToRoomId).IsRequired(false);

        builder.HasIndex(a => a.RequestCode).IsUnique();

        //Relationship
        builder.HasOne(x => x.ToRoom)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.ToRoomId);
        
        // Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.AssignedTo);

        builder.HasMany(x => x.TransportationDetails)
            .WithOne(x => x.Transportation)
            .HasForeignKey(x => x.TransportationId);
    }
}