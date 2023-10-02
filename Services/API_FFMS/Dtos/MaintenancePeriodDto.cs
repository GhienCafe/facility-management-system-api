using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos
{
    public class MaintenancePeriodCreateDto
    {
        public Guid AssetTypeId { get; set; }
        public int Period { get; set; }
        public DateTime SpecificDate { get; set; }
        public Guid? AssignedTo { get; set; }
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
