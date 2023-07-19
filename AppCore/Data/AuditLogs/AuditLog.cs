using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Data.AuditLogs;

public class AuditLog : BaseEntity
{
    public ActivityType Type { get; set; }
    public string Ip { get; set; }
    public string Device { get; set; }

    public string Method { get; set; }

    //
    public string PrimaryKey { get; set; }
    public string TableName { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public string AffectedColumns { get; set; }
}

public enum ActivityType
{
    Get = 1,
    Create = 3,
    Update = 4,
    Delete = 5,
    None = 6
}

public class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog");
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Method).IsRequired(false);
        builder.Property(x => x.Ip).IsUnicode(false).IsRequired();
        builder.Property(x => x.Device).IsUnicode(false).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime").IsRequired();
        //
        builder.Property(x => x.TableName).IsRequired(false);
        builder.Property(x => x.OldValues).IsRequired(false);
        builder.Property(x => x.NewValues).IsRequired(false);
        builder.Property(x => x.AffectedColumns).IsRequired(false);
        builder.Property(x => x.PrimaryKey).IsRequired(false);
    }
}