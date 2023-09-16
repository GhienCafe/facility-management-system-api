using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Notification : BaseEntity
{
    public Guid? UserId { get; set; }
    public bool IsSendAll { get; set; }
    public string? Title { get; set; }
    public string? ShortContent { get; set; }
    public string? Content { get; set; }
    
    //
    public virtual User? User { get; set; }
}

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.Property(x => x.UserId).IsRequired(false);
        builder.Property(x => x.IsSendAll).IsRequired();
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.ShortContent).IsRequired(false);
        builder.Property(x => x.Content).IsRequired();

        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId);
    }
}
