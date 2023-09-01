using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class ColorStatus : BaseEntity
{
    public string? Color { get; set; }
    
    //Relationship
    public virtual IEnumerable<Rooms>? Rooms { get; set; }
}

public class ColorStatusConfig : IEntityTypeConfiguration<ColorStatus>
{
    public void Configure(EntityTypeBuilder<ColorStatus> builder)
    {
        builder.Property(x => x.Color).IsRequired(false);
        //Relationship
        builder.HasMany(x => x.Rooms)
            .WithOne(x => x.ColorStatus)
            .HasForeignKey(x => x.ColorStatusId);
    }
}