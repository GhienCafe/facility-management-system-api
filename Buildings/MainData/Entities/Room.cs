using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Room : BaseEntity
{
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public Guid StatusId { get; set; }
    public Guid FloorId { get; set; }

    //Relationship
    public virtual Floor? Floors { get; set; }
    public virtual IEnumerable<RoomAsset>? RoomAssets { get; set; }
    public virtual RoomStatus? Status { get; set; }
}

public enum RoomTypeEnum
{
    Library = 1, TeaBreak=2, ClassRoom = 3, ItSupport = 4, GD_WS = 5, Hall = 6, Studio = 7, InstrumentClassroom = 8, WareHouse = 9
}

public class RoomConfig : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.Property(x => x.Area).IsRequired(false);
        builder.Property(x => x.PathRoom).IsRequired(false);
        builder.Property(x => x.RoomCode).IsRequired();
        builder.Property(x => x.RoomType).IsRequired();
        builder.Property(x => x.Capacity).IsRequired(false);
        builder.Property(x => x.StatusId).IsRequired();
        builder.Property(x => x.FloorId).IsRequired();
        
        //Attribute
        builder.HasIndex(x => x.RoomCode).IsUnique();
        
        //Relationship
        builder.HasOne(x => x.Floors)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.FloorId);

        builder.HasMany(x => x.RoomAssets)
            .WithOne(x => x.Room)
            .HasForeignKey(x => x.RoomId);
        
        builder.HasOne(x => x.Status)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.StatusId);
        
    }
}