using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Notification : BaseEntity
{
    public Guid? UserId { get; set; }
    public string? Title { get; set; }
    public string? ShortContent { get; set; }
    public string? Content { get; set; }
    public bool? IsRead { get; set; } 
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    
    public Guid? ItemId { get; set; }
    
    //
    public virtual User? User { get; set; }
    public virtual ActionRequest? Request { get; set; }
}

public enum NotificationType
{
    [Display(Name = "Nhiệm vụ")] Task = 1,
    [Display(Name = "Hệ thống")] System = 2,
    [Display(Name = "Thông tin")] Info = 3,
    [Display(Name = "Bảo trì")] Maintenance = 4,
    [Display(Name = "Thay thế")] Replacement = 5,
}

public enum NotificationStatus
{
    [Display(Name = "Chờ")] Waiting = 1,
    [Display(Name = "Đã gửi")] Sent = 2,
    [Display(Name = "Hủy")] Cancel = 3,
}

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.Property(x => x.UserId).IsRequired(false);
        builder.Property(x => x.IsRead).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.ShortContent).IsRequired(false);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.ItemId).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId);
        
        builder.HasOne(x => x.Request)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.ItemId);
    }
}
