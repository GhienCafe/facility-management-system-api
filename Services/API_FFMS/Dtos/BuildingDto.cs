using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class BuildingDto  : BaseDto
{
    public string? BuildingName { get; set; }
    public Guid? CampusId { get; set; }
}

public class BuildingDetailDto : BaseDto
{
    public string? BuildingName { get; set; }
    public Guid? CampusId { get; set; }
    public virtual Campus Campus { get; set; }
}

public class BuildingCreateDto
{
    public string? BuildingName { get; set; }
    public Guid? CampusId { get; set; }
}
public class BuildingUpdateDto
{
    public string? BuildingName { get; set; }
    public Guid? CampusId { get; set; }
}
public class BuildingQueryDto : BaseQueryDto
{
    public string? CampusName { get; }
    public string? BuildingName { get; }
}