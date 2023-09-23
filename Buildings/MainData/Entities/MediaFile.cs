using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class MediaFile : BaseEntity
{
    public string FileName { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string RawUri { get; set; } = null!;
    public string Uri { get; set; } = null!;
    public string Extensions { get; set; } = null!;
    public FileType FileType { get; set; }
    public string Content { get; set; } = null!;
    
    public Guid? ItemId { get; set; }
}

public enum FileType
{
    Image = 1,
    Video = 2,
    File = 3
}

public class MediaFileConfig : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("MediaFiles");
        builder.Property(a => a.FileName).IsRequired();
        builder.Property(a => a.Key).IsRequired();
        builder.Property(a => a.RawUri).IsRequired(false);
        builder.Property(a => a.Uri).IsRequired();
        builder.Property(a => a.Extensions).IsRequired();
        builder.Property(a => a.FileType).IsRequired();
        builder.Property(a => a.Content).IsRequired(false);
        builder.Property(a => a.ItemId).IsRequired(false);
    }
}