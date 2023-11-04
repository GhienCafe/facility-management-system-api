using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class VirtualizeFloorDto : BaseDto
{
    public string? FloorMap { get; set; }
    public string? FloorName { get; set; }
    public int? FloorNumber { get; set; }
    public string? Description { get; set; }
    public double? TotalArea { get; set; }
    public Guid? BuildingId { get; set; }
}

public class VirtualizeRoomDto: BaseDto
{
    public string? RoomName { get; set; }
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public Guid? RoomTypeId { get; set; }
    public int? Capacity { get; set; }
    public Guid StatusId { get; set; }
    public Guid FloorId { get; set; }
    public string? Description { get; set; }
    
    public int? TotalAssets { get; set; }
    public int? TotalDamagedAssets { get; set; }
    public int? TotalNormalAssets { get; set; }
    public int? TotalRepairAssets { get; set; }
    public int? TotalNeedInspectionAssets { get; set; }
    public int? TotalInMaintenanceAssets { get; set; }
    public int? TotalInReplacementAssets { get; set; }
    public int? TotalInTransportationAssets { get; set; }
    public int? TotalOtherAssets { get; set; }
    
    public RoomStatusDto? Status { get; set; }
    public RoomTypeDto? RoomType { get; set; }
}

public class VirtualizeRoomQueryDto : BaseQueryDto
{
    public Guid FloorId { get; set; }
}

public class VirtualizeQueryDto : BaseQueryDto
{
}

public class VirtualDashboard
{
    public int TotalBuilding { get; set; }
    public int TotalRoom { get; set; }
    public int TotalUser { get; set; }
    public int TotalAsset { get; set; }
    
}