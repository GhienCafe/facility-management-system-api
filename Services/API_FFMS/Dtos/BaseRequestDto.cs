﻿using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class BaseRequestDto : BaseDto
{
    public Guid AssetId { get; set; }
    public string RequestCode { get; set; } = null!;
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public bool IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }    
    public Guid? TypeId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class BaseRequestCreateDto
{
    public Guid AssetId { get; set; }
    public string RequestCode { get; set; } = null!;
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public bool IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }    
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class BaseRequestQueryDto : BaseQueryDto
{
    public Guid? AssetId { get; set; }
    public RequestStatus? Status { get; set; }
    public bool? IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }    
}

public class BaseRequestUpdateDto
{
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public Guid? AssignedTo { get; set; }    
}