using System.ComponentModel.DataAnnotations;
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class ReplacementDto : BaseDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public EnumValue? Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? NewAssetId { get; set; }
    public UserDto? PersonInCharge { get; set; }
    public AssetDto? Asset { get; set; }
}

public class CreateReplacementDto
{
    [Required(ErrorMessage = "RequestedDate is required")]
    [FutureDate(ErrorMessage = "Scheduled Date must be in the future")]
    public DateTime? RequestedDate { get; set; }
    
    public string? Description { get; set; }
    
    public string? Note { get; set; }
    public string? Reason { get; set; }

    [Required(ErrorMessage = "người gắn phải được yêu cầu")]
    public Guid AssignedTo { get; set; } = Guid.Empty;

    [Required(ErrorMessage = "tài sản phải được yêu cầu")]
    public Guid AssetId { get; set; } = Guid.Empty;

    [Required(ErrorMessage = "Tai sản mới phải được yêu cầu")]
    public Guid? NewAssetId { get; set; } = Guid.Empty;
}


public class UpdateReplacementDto
{
    [FutureDate(ErrorMessage = "Dự liệu của ngày yêu cầu phải nằm ở tương lai")]
    public DateTime? RequestedDate { get; set; } = DateTime.Now;

    public DateTime? CompletionDate { get; set; } = DateTime.Now;
    public string? Description { get; set; } = string.Empty;
    public string? Note { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public ActionStatus? Status { get; set; }
    public Guid? AssignedTo { get; set; } = Guid.Empty;
    public Guid? AssetId { get; set; } = Guid.Empty;
    public Guid? NewAssetId { get; set; } = Guid.Empty;
}

public class DetailReplacementDto : BaseDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public EnumValue Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? NewAssetId { get; set; }
    public UserDto? PersonInCharge { get; } = null;
    public AssetDto? Asset { get; } = null;
    public AssetDto? NewAsset { get; } = null;
}

public class QueryReplacementDto : BaseQueryDto
{
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public ActionStatus? Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? NewAssetId { get; set; }
}


[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class QuantityGreaterThanZeroAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int quantity && quantity > 0)
        {
            return ValidationResult.Success!;
        }

        return new ValidationResult("Số lượng phải lớn hơn 0");
    }
}
