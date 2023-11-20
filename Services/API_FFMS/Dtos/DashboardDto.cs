using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class DashboardDto
{
    public Double TotalQuantity { get; set; } = 0;
    public Double TotalInUsed { get; set; }= 0;
    public Double TotalNotUsed { get; set; }= 0;
    public Double TotalMaintenance { get; set; }= 0;
    public Double TotalRepair { get; set; }= 0;
    public Double TotalTransportation { get; set; }= 0;
    public Double TotalReplacement { get; set; }= 0;
    public Double TotalNeedInspection { get; set; }=0;
    public List<AssetTypeDashboardDto>? AssetType { get; set; }
}

public class AssetStatisticDto
{
    public double? TotalQuantity { get; set; }
    public double? TotalOperational { get; set; }
    public double? TotalNotUsed { get; set; }
    public double? TotalMaintenance { get; set; }
    public double? TotalRepair { get; set; }
    public double? TotalTransportation { get; set; }
    public double? TotalReplacement { get; set; }
    public double? TotalNeedInspection { get; set; }
}

public class AssetStatisticQueryDto
{
    public Unit Unit { get; set; } = Unit.Individual;
    public bool? IsRent {  get; set; }
}

public class ModelStatisticQueryDto
{
    public int? Month { get; set; } = 6;
}

public class AssetTypeStatisticDto
{
    public string? TypeName { get; set; }
    public List<ModelStatisticDto>? ModelStatistic { get; set; }

}

public class ModelStatisticDto
{
    public string? ModelName { get; set; }
    public int? TotalRepair { get; set; }
}

public class AssetTypeDashboardDto
{
    public Guid? TypeId { get; set; }
    public string? TypeName { get; set; }
    public Double Quantity { get; set; }
    public Double InUsed { get; set; }
    public Double NotUsed { get; set; }
    public Double Maintenance { get; set; }
    public Double Repair { get; set; }
    public Double Transportation { get; set; }
    public Double Replacement { get; set; }
    public Double NeedInspection { get; set; }
}

public class AssetDashBoardInformation
{
    public AssetStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public int Total { get; set; } 
    public double PercentPerTotal { get; set; } 
}  

public class TaskDashBoardInformation
{
    public RequestType? Type { get; set; }
    public EnumValue? TypeObj { get; set; }
    public IEnumerable<TaskBasedOnMonthDto>? TaskData { get; set; }
}

public class TaskDashBoardInformationForBinding
{
    public RequestType? Type { get; set; }
    public EnumValue? TypeObj { get; set; }
    public int Total { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public class TaskBasedOnMonthDto
{
    public int Total { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public class TaskBasedOnStatusDto
{
    public int Count { get; set; }
    public float Percent { get; set; }
    public EnumValue? Status { get; set; }
}

public class TaskBasedOnStatusDashboardDto
{
    public EnumValue? Type { get; set; }
    public int Total { get; set; }
    public IEnumerable<TaskBasedOnStatusDto>? Data { get; set; }
}

public class TaskStatisticDto
{
    public TaskStatisticDetailDto? AssetCheckTask {  get; set; }
    public TaskStatisticDetailDto? RepairTask { get; set; }
    public TaskStatisticDetailDto? ReplaceTask { get; set; }
    public TaskStatisticDetailDto? TransportTask { get; set; }
    public TaskStatisticDetailDto? MaintenanceTask { get; set; }
    public TaskStatisticDetailDto? InventoryCheckTask { get; set; }
}

public class TaskStatisticDetailDto
{
    public int? Total { get; set; }
    public int? Process { get; set; }
    public int? Complete { get; set; }
    public int? Waiting { get; set; }
    public int? Reported { get; set; }
}