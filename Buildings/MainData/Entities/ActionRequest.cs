using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class ActionRequest : BaseEntity
{
    public string RequestCode { get; set; } = null!;
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestType? RequestType { get; set; }
    public RequestStatus? RequestStatus { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; } // Results
    public bool IsInternal { get; set; }
    public Guid? AssignedTo { get; set; }
    
    // Relationship
    public User? PersonInCharge { get; set; }
    public IEnumerable<Transportation>? Transportations { get; set; }
    public IEnumerable<Maintenance>? Maintenances { get; set; }
    public IEnumerable<Replacement>? Replacements { get; set; }
    public IEnumerable<Repairation>? Repairations { get; set; }
    public IEnumerable<AssetCheck>? AssetChecks { get; set; }
    public IEnumerable<Notification>? Notifications { get; set; }
}

public enum RequestType
{
    [Display(Name = "Kiểm trang trình trạng trang thiết bị")]
    StatusCheck = 1,
    [Display(Name = "Bảo trì, nâng cấp trang thiết bị")]
    Maintenance = 2,
    [Display(Name = "Sửa chữa trang thiết bị")]
    Repairation = 3,
    [Display(Name = "Thay thế trang thiết bị")]
    Replacement = 4,
    [Display(Name = "Điều chuyển trang thiết bị")]
    Transportation = 5
}

public enum RequestStatus
{
    [Display(Name = "Chưa bắt đầu")]
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

public class RequestConfig : IEntityTypeConfiguration<ActionRequest>
{
    public void Configure(EntityTypeBuilder<ActionRequest> builder)
    {
        builder.ToTable("Requests");
        builder.Property(a => a.RequestCode).IsRequired();
        builder.Property(a => a.RequestDate).IsRequired(false);
        builder.Property(a => a.CompletionDate).IsRequired(false);
        builder.Property(a => a.RequestType).IsRequired(false);
        builder.Property(a => a.RequestStatus).IsRequired(false);
        builder.Property(a => a.Description).IsRequired(false);
        builder.Property(a => a.Notes).IsRequired(false);
        builder.Property(a => a.IsInternal).IsRequired();
        builder.Property(a => a.AssignedTo).IsRequired(false);

        builder.HasOne(x => x.PersonInCharge)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasMany(x => x.Notifications)
            .WithOne(x => x.Request)
            .HasForeignKey(x => x.ItemId);
    }
}