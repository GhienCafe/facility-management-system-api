using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;

public class InventoryCheckDto
{
}

public class InventoryCheckCreateDto
{
    public required Guid InventoryCheckConfigId { get; set; }
    public required List<Guid> AssetIds { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public bool IsInternal { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;

    public required Guid AssignedTo { get; set; }

    public required Guid RoomId { get; set; }
    //public MediaFileCreateDto? RelatedFile { get; set; }
}

public class InventoryCheckConfigDto : BaseDto
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
}

public class InventoryCheckConfigCreateDto
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
}

public class InventoryCheckConfigUpdateDto
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
}

public class InventoryCheckConfigQueryDto : BaseQueryDto
{
    public int? CheckPeriod { get; set; }
    public string? Description { get; set; }
}