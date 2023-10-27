using AppCore.Configs;
using AppCore.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_FFMS.Dtos;

public class AssetExportDtos
{
    [Display(Name = "Tên trang thiết bị")]
    public string AssetName { get; set; } = null!;

    [Display(Name = "Mã trang thiết bị")]
    public string? AssetCode { get; set; }

    [Display(Name = "Loại thiết bị")]
    public string? Category { get; set; }

    [Display(Name = "Nhóm")]
    public string? TypeName { get; set; }

    [Display(Name = "Có thể di chuyển")]
    public string? IsMovable { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Năm sản xuất")]
    public int? ManufacturingYear { get; set; }

    [Display(Name = "Phòng hiện tại")]
    public string? CurrentRoom { get; set; }

    [Display(Name = "Số serial")]
    public string? SerialNumber { get; set; }

    [Display(Name = "Số lượng (Cái)")]
    public double Quantity { get; set; }

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Đang thuê")]
    public string? IsRented { get; set; }

    [Display(Name = "Nhãn hiệu")]
    public string? ModelName { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    [Display(Name = "Thời gian bảo dưỡng gần nhất")]
    public DateTime? LastMaintenanceTime { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    [Display(Name = "Thời gian kiểm tra gần nhất")]
    public DateTime? LastCheckedDate { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    [Display(Name = "Ngày bắt đầu sử dụng")]
    public DateTime? StartDateOfUse { get; set; }
}

public class TaskExportDto
{
    [Display(Name = "Mã yêu cầu")]
    public string? RequestCode { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    [Display(Name = "Ngày yêu cầu")]
    public DateTime? RequestDate { get; set; }

    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    [Display(Name = "Ngày hoàn thành")]
    public DateTime? CompletionDate { get; set; }

    [Display(Name = "Tên thiết bị")]
    public string? AssetName { get; set; }

    [Display(Name = "Loại thiết bị")]
    public string? Category { get; set; }

    [Display(Name = "Nhóm thiết bị")]
    public string? TypeName { get; set; }

    [Display(Name = "Thay thế bởi")]
    public string? NewAsset { get; set; }

    [Display(Name = "Phòng")]
    public string? Location { get; set; }

    [Display(Name = "Chịu trách nhiệm")]
    public string? AssignedTo { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Chú thích")]
    public string? Notes { get; set; }

    [Display(Name = "Kết quả")]
    public string? Result { get; set; }

    [Display(Name = "Phạm vi yêu cầu")]
    public string? IsInternal { get; set; }

    [Display(Name = "Vận chuyển đến")]
    public string? ToRoom { get; set; }

    [Display(Name = "Danh sách thiết bị")]
    public List<AssetTransportExportDto>? Assets { get; set; }
}