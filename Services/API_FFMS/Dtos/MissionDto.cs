using AppCore.Models;

namespace API_FFMS.Dtos;

public class MissionDto : BaseDto
{
    public virtual MaintenanceDto? MaintenanceMission { get; set; }
    public virtual ReplacementDto? ReplacementMission { get; set; }
    public virtual RepairationDto? RepairMission { get; set; }
    public virtual TransportDto? TransportMission { get; set; }
}

public class QueryMissionDto : BaseQueryDto
{
    public DateTime FromDate { get; }
    public DateTime ToDate { get; }
}