using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Rooms:BaseEntity
{
    public double? Area { get; set; }
    public string PathRoom { get; set; }
    public string RoomNumber { get; set; }
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public StatusEnum Status { get; set; }
    public Guid FloorId { get; set; }
    public Guid ColorStatusId { get; set; }

    //Relationship
    public virtual ColorStatus? ColorStatus { get; set; }
    public virtual Floors? Floors { get; set; }
}
public enum StatusEnum  
{
    ClassInSession=1,UnderMaintenance=2,NotAvailable,
}

public enum RoomTypeEnum
{
    Library = 1, TeaBreak=2, ClassRoom = 3, ItSupport = 4, GD_WS = 5, HallA=6, HallB=7, HallC=8, Studio = 9, LB=10 , WareHouse = 11
}

public class RoomsConfig : IEntityTypeConfiguration<Rooms>
{
    public void Configure(EntityTypeBuilder<Rooms> builder)
    {
        builder.Property(x => x.Area).IsRequired(false);
        builder.Property(x => x.PathRoom).IsRequired();
        builder.Property(x => x.RoomNumber).IsRequired();
        builder.Property(x => x.RoomType).IsRequired();
        builder.Property(x => x.Capacity).IsRequired(false);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.FloorId).IsRequired();
        builder.Property(x => x.ColorStatusId).IsRequired();
        //Relationship
        builder.HasOne(x => x.Floors)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.FloorId);
        builder.HasOne(x => x.ColorStatus)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.ColorStatusId);
    }
}