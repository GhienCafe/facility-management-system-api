using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class BaseRequest : BaseEntity
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
    
    //Relationship
    public virtual Asset? Asset { get; set; }
    public virtual User? User { get; set; }
}

public enum RequestStatus
{
    [Display(Name = "Đã gửi yêu cầu")]
    NotStarted = 1,
    [Display(Name = "Đang trong quá trình thực hiện")]
    InProgress = 2,
    [Display(Name = "Đã hoàn thành")]
    Completed = 3,
    [Display(Name = "Yêu cầu không thể thực hiện")]
    CantDo = 4,
    [Display(Name = "Đã hủy yêu cầu")]
    Cancelled = 5,
    [Display(Name = "Khác")]
    Others = 6,
}

public class BaseRequestConfig : IEntityTypeConfiguration<BaseRequest>
{
    public void Configure(EntityTypeBuilder<BaseRequest> builder)
    {
        builder.Property(a => a.RequestCode).IsRequired();
        builder.Property(a => a.RequestDate).IsRequired(false);
        builder.Property(a => a.CompletionDate).IsRequired(false);
        builder.Property(a => a.Status).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        builder.Property(a => a.Notes).IsRequired(false);
        builder.Property(a => a.IsInternal).IsRequired();
        builder.Property(a => a.AssignedTo).IsRequired(false);
        
        builder.HasIndex(a => a.RequestCode).IsUnique();
    }
}