using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class RoomDto
{
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public Guid StatusId { get; set; }
    public Guid FloorId { get; set; }
}

public class RoomDetailDto: BaseDto
{
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public Guid StatusId { get; set; }
    public Guid FloorId { get; set; }
}

public class RoomCreateDto
{
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public Guid StatusId { get; set; }
    public Guid FloorId { get; set; }
}

public class RoomUpdateDto
{
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string RoomCode { get; set; } = null!;
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public Guid StatusId { get; set; }
    public Guid FloorId { get; set; }
}
public class RoomQueryDto : BaseQueryDto
{
    public string? PathRoom { get; }
    public string? RoomCode { get; }
    public string? RoomType { get; }
}