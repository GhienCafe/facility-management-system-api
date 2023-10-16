using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class BaseRequest : BaseEntity
{
    public string RequestCode { get; set; } = null!;
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? Result { get; set; }
    public bool IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }

    //Relationship
    public virtual Asset? Asset { get; set; }
    public virtual User? User { get; set; }
}

public enum RequestStatus
{
    [Display(Name = "Chưa bắt đầu")]
    NotStart = 2,
    [Display(Name = "Đang xử lý")]
    InProgress = 1,
    [Display(Name = "Đã báo cáo")]
    Reported = 2,
    [Display(Name = "Đã hoàn thành")]
    Done = 3,
    [Display(Name = "Đã hủy")]
    Cancelled = 4,
    [Display(Name = "Khác")]
    Others = 5,
}

public enum RequestType
{
    [Display(Name = "Kiểm tra tình trạng")]
    StatusCheck = 1,
    [Display(Name = "Bảo trì, nâng cấp")]
    Maintenance = 2,
    [Display(Name = "Sửa chữa")]
    Repairation = 3,
    [Display(Name = "Thay thế")]
    Replacement = 4,
    [Display(Name = "Điều chuyển")]
    Transportation = 5
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
        builder.Property(a => a.Result).IsRequired(false);
        
        builder.HasIndex(a => a.RequestCode).IsUnique();
    }
}