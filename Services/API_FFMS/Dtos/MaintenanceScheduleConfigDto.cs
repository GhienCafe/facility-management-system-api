using AppCore.Models;

namespace API_FFMS.Dtos;

public class MaintenanceScheduleConfigDto : BaseDto
{
    public string? Code { get; set; }
    public int RepeatIntervalInMonths { get; set; } 
    public string? Description { get; set; }
}

public class MaintenanceScheduleConfigCreateDto
{
    public int RepeatIntervalInMonths { get; set; } 
    public string? Description { get; set; }
    
    public IEnumerable<Guid>? AssetIds { get; set; }
}

public class MaintenanceScheduleConfigUpdateDto
{
    public int? RepeatIntervalInMonths { get; set; } 
    public string? Description { get; set; }
}

public class MaintenanceScheduleConfigDetailDto : BaseDto
{
    public string? Code { get; set; }
    public int RepeatIntervalInMonths { get; set; } 
    public string? Description { get; set; }
}

public class MaintenanceScheduleConfigQueryDto : BaseQueryDto
{
}

