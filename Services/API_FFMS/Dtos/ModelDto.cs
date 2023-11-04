using AppCore.Models;

namespace API_FFMS.Dtos;

public class ModelDto : BaseDto
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
    public int? MaintenancePeriodTime { get; set; }
    public Guid? BrandId { get; set; }
    public BrandDto? Brand { get; set; }
}

public class ModelCreateDto
{
    public Guid BrandId { get; set; }
    public string? ModelName { get; set; }
    public string? Description { get; set; }
    public int? MaintenancePeriodTime { get; set; }
}

public class ModelUpdateDto
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
    public int? MaintenancePeriodTime { get; set; }
    public Guid? BrandId { get; set; }
}

public class ModelQueryDto : BaseQueryDto
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
}