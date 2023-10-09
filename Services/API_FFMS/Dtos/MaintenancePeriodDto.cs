using AppCore.Models;

namespace API_FFMS.Dtos
{
    public class MaintenancePeriodCreateDto
    {
        public List<Guid>? AssetId { get; set; }
        public int Period { get; set; } //Month
    }
    public class MaintenancePeriodUpdateDto
    {
        public List<Guid>? AssetId { get; set; }
        public int Period { get; set; } //Month
        public DateTime? LastMaintenanceTime { get; set; }
        public DateTime NextMaintenance { get; set; } //NextMaintenance = LastMaintenance + Period -> LastMaintenance
    }

    public class MaintenanceScheduleConfigDetailDto : BaseDto
    {
        public Guid AssetTypeId { get; set; }
        public int Period { get; set; }
        public DateTime SpecificDate { get; set; }
        public Guid? AssignedTo { get; set; }
        public UserDto? PersonInCharge { get; set; }
        public AssetDto? Asset { get; set; }
    }
}
