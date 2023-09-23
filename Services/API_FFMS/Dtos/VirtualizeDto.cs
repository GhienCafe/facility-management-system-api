using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class VirtualizeDto:BaseDto
{
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public Guid? RoomTypeId { get; set; }
    public int? Capacity { get; set; }
    public string? StatusName { get; set; }
    public string? FloorNumber { get; set; }
}

public class VirtualizeQueryDto : BaseQueryDto
{
    public string? FloorNumber { get; set; }
}