using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MainData.Entities;

public class RoomStatus : BaseEntity
{
    public string StatusName { get; set; } = null!;
    public RoomStatuses Status { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    
    //
    public virtual IEnumerable<Room>? Rooms { get; set; }
}

public enum RoomStatuses
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
    OutOfOrder = 8,

    [Display(Name = "Phòng thiếu trang thiết bị cần thiết")]
    MissingAsset = 9

}

public class RoomStatusConfig : IEntityTypeConfiguration<RoomStatus>
{
    public void Configure(EntityTypeBuilder<RoomStatus> builder)
    {
        builder.ToTable("RoomStatus");
        builder.Property(x => x.StatusName).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Color).IsRequired();
        
        //Relationship
        builder.HasMany(x => x.Rooms)
            .WithOne(x => x.Status)
            .HasForeignKey(x => x.StatusId);
    }
}