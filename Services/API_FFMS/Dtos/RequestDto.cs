// using AppCore.Models;
// using MainData.Entities;
//
// namespace API_FFMS.Dtos;
// public class ActionRequestDto : BaseDto
// {
//     public string RequestCode { get; set; } = null!;
//     public DateTime? RequestDate { get; set; }
//     public DateTime? CompletionDate { get; set; }
//     public RequestType? RequestType { get; set; }
//     public EnumValue? RequestTypeObj { get; set; }
//     public RequestStatus? RequestStatus { get; set; }
//     public EnumValue? RequestStatusObj { get; set; }
//     public string? Description { get; set; }
//     public string? Notes { get; set; } // Results
//     public bool IsInternal { get; set; }
//     public Guid? AssignedTo { get; set; }
//     public UserDto? AssignedPerson { get; set; }
// }
//
// public class ActionRequestDetailDto : BaseDto
// {
//     public string RequestCode { get; set; } = null!;
//     public DateTime? RequestDate { get; set; }
//     public DateTime? CompletionDate { get; set; }
//     public RequestType? RequestType { get; set; }
//     public EnumValue? RequestTypeObj { get; set; }
//     public RequestStatus? RequestStatus { get; set; }
//     public EnumValue? RequestStatusObj { get; set; }
//     public string? Description { get; set; }
//     public string? Notes { get; set; } // Results
//     public bool IsInternal { get; set; }
//     public Guid? AssignedTo { get; set; }
//     public UserDto? AssignedPerson { get; set; }
// }
//
// public class ActionRequestUpdateDto
// {
//     public string? RequestCode { get; set; } 
//     public DateTime? RequestDate { get; set; }
//     public DateTime? CompletionDate { get; set; }
//     //public RequestType? RequestType { get; set; }
//     public RequestStatus? RequestStatus { get; set; }
//     public string? Description { get; set; }
//     public string? Notes { get; set; } // Results
//     public bool? IsInternal { get; set; }
//     public Guid? AssignedTo { get; set; }
// }
//
// public class ActionRequestCreateDto
// {
//     public string RequestCode { get; set; } = null!;
//     public DateTime? RequestDate { get; set; }
//     public DateTime? CompletionDate { get; set; }
//     public RequestType? RequestType { get; set; }
//     public RequestStatus? RequestStatus { get; set; }
//     public string? Description { get; set; }
//     public string? Notes { get; set; } // Results
//     public bool IsInternal { get; set; }
//     public Guid? AssignedTo { get; set; }
//     public IEnumerable<TransportCreateInRequestDto>? Transportations { get; set; }
//     public IEnumerable<MaintenanceCreateInRequestDto>? Maintenances { get; set; }
//     public IEnumerable<RepairCreateInRequestDto>? Repairations { get; set; }
//     public IEnumerable<ReplaceCreateInRequestDto>? Replacements { get; set; }
// }
//
// public class TransportCreateInRequestDto
// {
//     public int? Quantity { get; set; }
//     public Guid? AssetId { get; set; }
//     public Guid? ToRoomId { get; set; }
// }
//
// public class MaintenanceCreateInRequestDto
// {
//     public Guid? AssetId { get; set; }
//     public string? Note { get; set; }
// }
//
// public class RepairCreateInRequestDto
// {
//     public Guid? AssetId { get; set; }
//     public string? Description { get; set; }
//     public string? Note { get; set; }
// }
//
// public class ReplaceCreateInRequestDto
// {
//     public Guid? AssetId { get; set; }
//     public Guid? NewAssetId { get; set; }
// }
//
// public class ActionRequestQuery : BaseQueryDto
// {
//     public RequestType? RequestType { get; set; }
//     public RequestStatus? RequestStatus { get; set; }
//     public Guid? AssignedTo { get; set; }
//     public bool? IsInternal { get; set; }
// }