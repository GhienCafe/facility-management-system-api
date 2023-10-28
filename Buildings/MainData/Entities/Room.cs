using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Room : BaseEntity
{
    public string? RoomName { get; set; }
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public RoomState? State { get; set; }
    public Guid? RoomTypeId { get; set; }
    public int? Capacity { get; set; }
    public Guid StatusId { get; set; }
    public Guid FloorId { get; set; }
    public string? Description { get; set; }
    
    //Relationship
    public virtual Floor? Floors { get; set; }
    public virtual RoomType? RoomType { get; set; }
    public virtual IEnumerable<RoomAsset>? RoomAssets { get; set; }
    public virtual RoomStatus? Status { get; set; }
    public virtual IEnumerable<Transportation>? Transportations { get; set; }
}

public enum RoomState
{
    [Display(Name = "Phòng hoạt động bình thường")]
    Operational = 1,

    [Display(Name = "Phòng không thể sử dụng")]
    Inactive = 2,

    [Display(Name = "Phòng có thiết bị đang trong quá trình bảo dưỡng")]
    Maintenance = 3,

    [Display(Name = "Phòng có thiết bị đang trong quá trình sửa chữa")]
    Repair = 4,

    [Display(Name = "Phòng có thiết bị đang trong quá trình kiểm tra")]
    NeedInspection = 5,

    [Display(Name = "Phòng có thiết bị đang trong quá trình thay thế")]
    Replacement = 6,

    [Display(Name = "Phòng có thiết bị đang trong quá trình điều chuyển")]
    Transportation = 7,

    [Display(Name = "Phòng có thiết đang bị hư hại")]
    Damaged = 8,

    [Display(Name = "Phòng thiếu trang thiết bị cần thiết")]
    MissingAsset = 9
}

public class RoomConfig : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.Property(x => x.RoomName).IsRequired(false);
        builder.Property(x => x.Area).IsRequired(false);
        builder.Property(x => x.PathRoom).IsRequired(false);
        builder.Property(x => x.RoomCode).IsRequired();
        builder.Property(x => x.RoomTypeId).IsRequired(false);
        builder.Property(x => x.Capacity).IsRequired(false);
        builder.Property(x => x.StatusId).IsRequired();
        builder.Property(x => x.FloorId).IsRequired();
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.State).IsRequired(false);
        
        //Attribute
        builder.HasIndex(x => x.RoomCode).IsUnique();
        
        //Relationship
        builder.HasOne(x => x.Floors)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.FloorId);
        
        builder.HasOne(x => x.RoomType)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.RoomTypeId);

        builder.HasMany(x => x.RoomAssets)
            .WithOne(x => x.Room)
            .HasForeignKey(x => x.RoomId);
        
        builder.HasOne(x => x.Status)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.StatusId);

        builder.HasMany(x => x.Transportations)
            .WithOne(x => x.ToRoom)
            .HasForeignKey(x => x.ToRoomId);
        
    }
}