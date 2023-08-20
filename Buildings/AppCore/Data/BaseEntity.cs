using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Data;

public class BaseEntity
{
    public Guid Id { get; set; }
    public Guid? CreatorId { get; set; }
    public Guid? EditorId { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EditedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class BaseConfig : IEntityTypeConfiguration<BaseEntity>
{
    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("newid()").IsRequired();
        builder.Property(x => x.CreatorId).IsRequired(false);
        builder.Property(x => x.EditorId).IsRequired(false);
        builder.Property(x => x.DeleterId).IsRequired(false);
        builder.Property(x => x.CreatedAt).HasColumnType("datetime").IsRequired();
        builder.Property(x => x.EditedAt).HasColumnType("datetime").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("datetime").IsRequired(false);
    }
}