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