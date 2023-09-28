using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class ExportTrackingDto : BaseDto
{
    public DateTime FromDateTracking { get; set; }
    public DateTime? ToDateTracking { get; set; }
    
    //Asset
    public Guid? AssetId { get; set; }
    public Guid? TypeId { get; set; }
    public string? AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public bool? IsMovable { get; set; }
    public AssetStatus? AssetStatus { get; set; }
    public DateTime? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double? Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime? LastMaintenanceTime { get; set; }
    
    //Room
    public string? RoomName { get; set; }
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string? RoomCode { get; set; } = null!;
    // public EnumValue? RoomType { get; set; }
    public Guid? RoomTypeId { get; set; }
    public int? Capacity { get; set; }
    public Guid? StatusId { get; set; }
    public Guid FloorId { get; set; }
    public RoomStatusDto RoomStatus { get; set; }
    public RoomTypeDto? RoomType { get; set; }
}

public class ExportQueryTrackingDto : BaseQueryDto
{
    
}