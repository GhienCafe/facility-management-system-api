using AppCore.Models;

namespace API_FFMS.Dtos;

public class ModelDto : BaseDto
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
}

public class ModelCreateDto
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
}

public class ModelUpdateDto
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
}

public class ModelQueryDto : BaseQueryDto
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
}