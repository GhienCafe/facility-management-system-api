using AppCore.Models;

namespace API_FFMS.Dtos;

public class RoomStatusDto : BaseDto
{
    public string StatusName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Color { get; set; }
}
public class RoomStatusDetailDto : BaseDto
{
    public string StatusName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class RoomStatusCreateDto
{
    public string StatusName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class RoomStatusUpdateDto
{
    public string StatusName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class RoomStatusQueryDto : BaseQueryDto
{
    public string? StatusName { get; set; }
}