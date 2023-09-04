using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class VirtualizeDto:BaseDto
{
    public double? Area { get; set; }
    public string PathRoom { get; set; }
    public string RoomNumber { get; set; }
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public StatusEnum Status { get; set; }
    public Guid FloorId { get; set; }
    public List<Rooms> RoomList { get; }
    public List<ColorStatus> ColorStatus { get; }
}

public class VirtualizeQueryDto : BaseQueryDto
{
    public string? FloorNumber { get; set; }
}