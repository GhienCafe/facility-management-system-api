using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class MaintenanceDto : BaseDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public EnumValue? Status { get; set; }
    public EnumValue? Type { get; set; }
    public Guid? AssignedTo { get; set; }
    public UserDto? PersonInCharge { get; set; }
    public Guid? AssetId { get; set; }
    public AssetDto? Asset { get; set; }
}

public class MaintenanceDetailDto : BaseDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public EnumValue? Status { get; set; }
    public EnumValue? Type { get; set; }
    public Guid? AssignedTo { get; set; }
    public UserDto? PersonInCharge { get; set; }
    public Guid? AssetId { get; set; }
    public AssetDto? Asset { get; set; }
}

public class MaintenanceCreateDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
}

public class MaintenanceUpdateDto
{
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
}

public class MaintenanceQueryDto : BaseQueryDto
{
    public DateTime? RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
}