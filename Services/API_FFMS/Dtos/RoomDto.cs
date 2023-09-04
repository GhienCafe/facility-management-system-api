using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class RoomDto
{
    public double? Area { get; set; }
    public string PathRoom { get; set; }
    public string RoomNumber { get; set; }
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public StatusEnum Status { get; set; }
    public Guid FloorId { get; set; }
}

public class RoomDetailDto: BaseDto
{
    public double? Area { get; set; }
    public string PathRoom { get; set; }
    public string RoomNumber { get; set; }
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public StatusEnum Status { get; set; }
    public Guid FloorId { get; set; }
    
    //Relationship
    public virtual Floors? Floor { get; set; }
}

public class RoomCreateDto
{
    public double? Area { get; set; }
    public string PathRoom { get; set; }
    public string RoomNumber { get; set; }
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public StatusEnum Status { get; set; }
    public Guid FloorId { get; set; }
}

public class RoomUpdateDto
{
    public double? Area { get; set; }
    public string PathRoom { get; set; }
    public string RoomNumber { get; set; }
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    public StatusEnum Status { get; set; }
    public Guid FloorId { get; set; }
}
public class RoomQueryDto : BaseQueryDto
{
    public string? PathRoom { get; }
    public string? RoomNumber { get; }
    public string? RoomType { get; }
}