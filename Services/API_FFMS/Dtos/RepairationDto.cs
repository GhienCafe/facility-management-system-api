using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos
{
    public class RepairationCreateDto
    {
        public DateTime RequestedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? Reason { get; set; }
        public RepairationStatus Status { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? AssetId { get; set; }
    }

    public class RepairationDetailDto : BaseDto
    {
        public DateTime RequestedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? Reason { get; set; }
        public RepairationStatus Status { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? AssetId { get; set; }
        public UserDto? PersonInCharge { get; set; }
        public AssetDto? Asset { get; set; }
    }

    public class RepairationDto : BaseDto
    {
        public DateTime RequestedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? Reason { get; set; }
        public RepairationStatus Status { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? AssetId { get; set; }
        public UserDto? PersonInCharge { get; set; }
        public AssetDto? Asset { get; set; }
    }

    public class RepairationUpdateDto
    {
        public DateTime? RequestedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? Reason { get; set; }
        //public RepairationStatus Status { get; set; }
        public Guid? AssignedTo { get; set; }
        //public Guid? AssetId { get; set; }
    }

    public class RepairationQueryDto : BaseQueryDto
    {
        public DateTime? RequestedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        //public string? Description { get; set; }
        public string? Note { get; set; }
        //public string? Reason { get; set; }
        public RepairationStatus? Status { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? AssetId { get; set; }
    }
}
