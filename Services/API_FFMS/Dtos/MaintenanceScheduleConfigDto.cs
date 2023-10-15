using AppCore.Models;

namespace API_FFMS.Dtos;

public class MaintenanceScheduleConfigDto : BaseDto
{
    public Guid AssetId { get; set; }
    public int Period { get; set; } 
    
    public AssetBaseDto? Asset { get; set; }
}