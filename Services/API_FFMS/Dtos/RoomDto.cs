using System.ComponentModel.DataAnnotations;
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class RoomDto : BaseDto
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
    [Required]
    public string RoomCode { get; set; } = null!;
    [Required]
    public RoomTypeEnum RoomType { get; set; }
    public int? Capacity { get; set; }
    [Required]
    public Guid StatusId { get; set; }
    [Required]
    public Guid FloorId { get; set; }
}

public class RoomUpdateDto
{
    public double? Area { get; set; }
    public string? PathRoom { get; set; }
    public string? RoomCode { get; set; }
    public RoomTypeEnum? RoomType { get; set; }
    public int? Capacity { get; set; }
    public Guid? StatusId { get; set; }
    public Guid? FloorId { get; set; }
}
public class RoomQueryDto : BaseQueryDto
{
    public double? FromArea { get; set; }
    public double? ToArea { get; set; }
    public double? FromCapacity { get; set; }
    public double? ToCapacity { get; set; }
    public string? Status { get; set; }
    public string? RoomCode { get; set; }
    public RoomTypeEnum? RoomType { get; set; }
}