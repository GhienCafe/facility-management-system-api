using AppCore.Models;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;

public class RoomTypeDto : BaseDto
{
    public string TypeName { get; set; } = null!;
    public string? Description { get; set; }
}

public class RoomTypeDetailDto : BaseDto
{
    public string TypeName { get; set; } = null!;
    public string? Description { get; set; }
    //public List<RoomIncludeDto>? RoomInclude { get; set; }
}

public class RoomIncludeDto
{
    public string? RoomName { get; set; }
    public string? RoomCode { get; set; }
}

public class RoomTypeQueryDto : BaseQueryDto
{
    public string? TypeName { get; set; }
}

public class RoomTypeCreateDto
{
    [Required]
    public string TypeName { get; set; } = null!;
    public string? Description { get; set; }
}

public class RoomTypeUpdateDto
{
    public string TypeName { get; set; } = null!;
    public string? Description { get; set; }
}