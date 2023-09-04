using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class VirtualizeDto:BaseDto
{
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public Guid StatusId { get; set; }
    public Guid FloorId { get; set; }
}

public class VirtualizeQueryDto : BaseQueryDto
{
    public string? FloorNumber { get; set; }
}