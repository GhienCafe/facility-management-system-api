using System.ComponentModel.DataAnnotations;
using API_FFMS.Dtos;
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos
{
    public class ExportTrackingRoomDto : BaseDto
    {
        [Display(Name = "Tên phòng")]
        public string? RoomName { get; set; }

        [Display(Name = "Diện tích")]
        public double? Area { get; set; }

        [Display(Name = "Đường dẫn phòng")]
        public string? PathRoom { get; set; }

        [Display(Name = "Mã phòng")]
        public string RoomCode { get; set; } = null!;

        // public EnumValue? RoomType { get; set; }

        public Guid? RoomTypeId { get; set; }
        [Display(Name = "Sức chứa")]
        public int? Capacity { get; set; }
        public Guid StatusId { get; set; }
        public Guid FloorId { get; set; }
        [Display(Name = "Tên loại phòng")]
        public string? TypeName { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public RoomStatusDto? Status { get; set; }

        public RoomTypeDto? RoomType { get; set; }

        public FloorBaseDto? Floor { get; set; }
        [Display(Name = "Trạng thái phòng")]
        public string? StatusName { get; set; }
    }
}

public class QueryRoomExportDto : BaseQueryDto
{
    public double? FromArea { get; set; }
    public double? ToArea { get; set; }
    public double? FromCapacity { get; set; }
    public double? ToCapacity { get; set; }
    public Guid? StatusId { get; set; }

    public Guid? RoomTypeId { get; set; }

//public RoomTypeEnum? RoomType { get; set; }
    public Guid? FloorId { get; set; }
    
    public AssetStatus? AssetStatus { get; set; }
}

public class AssetExportDto : BaseDto
{
    [Display(Name = "Tên tài sản")]
    public string AssetName { get; set; } = null!;

    [Display(Name = "Mã tài sản")]
    public string? AssetCode { get; set; }

    [Display(Name = "Có thể di chuyển")]
    public bool IsMovable { get; set; }

    [Display(Name = "Trạng thái tài sản")]
    public AssetStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }

    [Display(Name = "Năm sản xuất")]
    public int? ManufacturingYear { get; set; }

    [Display(Name = "Số serial")]
    public string? SerialNumber { get; set; }

    [Display(Name = "Số lượng (Cái)")]
    public double Quantity { get; set; }

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Thời gian bảo dưỡng gần nhất")]
    public DateTime? LastMaintenanceTime { get; set; }

    public Guid? TypeId { get; set; }

    public Guid? ModelId { get; set; }

    [Display(Name = "Đang thuê")]
    public bool? IsRented { get; set; }
    [Display(Name = "Loại tài sản")]
    public string? TypeName { get; set; }
    [Display(Name = "Tên nhãn hiệu")]
    public string? ModelName { get; set; }

    [Display(Name = "Thời gian kiểm tra gần nhất")]
    public DateTime? LastCheckedDate { get; set; }

    [Display(Name = "Ngày bắt đầu sử dụng")]
    public DateTime? StartDateOfUse { get; set; }
}

public class QueryAssetExportDto : BaseQueryDto
{
    public AssetStatus? Status { get; set; }
    public bool? IsMovable { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public bool? IsRented { get; set; }
}