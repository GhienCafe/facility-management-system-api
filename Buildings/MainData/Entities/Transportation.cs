using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Transportation : BaseEntity
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public TransportationStatus Status { get; set; }
    public int? Quantity { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? ToRoomId { get; set; } 

    public virtual Asset? Asset { get; set; }
    public virtual Room? ToRoom { get; set; }
    public virtual User? PersonInCharge { get; set; }
    //public virtual User? Creator { get; set; }
}

public enum TransportationStatus
{
    [Display(Name = "Chưa bắt đầu")]
    NotStarted = 1,
    [Display(Name = "Chưa thực hiện")]
    InProgress = 2,
    [Display(Name = "Hoa thành")]
    Completed = 3,
    [Display(Name = "Hủy")]
    Cancelled = 4,
}

public class TransportationConfig : IEntityTypeConfiguration<Transportation>
{
    public void Configure(EntityTypeBuilder<Transportation> builder)
    {
        builder.ToTable("Transportations");
        builder.Property(x => x.RequestedDate).IsRequired();
        builder.Property(x => x.CompletionDate).IsRequired(false);
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Note).IsRequired(false);
        builder.Property(x => x.Quantity).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.AssignedTo).IsRequired(false);

        //Relationship

        // builder.HasOne(x => x.Creator)
        //     .WithMany(x => x.Transportations)
        //     .HasForeignKey(x => x.CreatorId);
        
        builder.HasOne(x => x.PersonInCharge)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.AssetId);
        
        builder.HasOne(x => x.ToRoom)
            .WithMany(x => x.Transportations)
            .HasForeignKey(x => x.ToRoomId);
    }
}