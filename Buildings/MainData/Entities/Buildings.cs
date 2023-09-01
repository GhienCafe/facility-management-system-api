using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Buildings : BaseEntity
{
    public string? BuildingName { get; set; }
    public Guid? CampusId { get; set; }
    //Relationship
    public virtual IEnumerable<Floors>? Floors { get; set; }
    public virtual Campus Campus { get; set; }
}

public class BuildingsConfig : IEntityTypeConfiguration<Buildings>
{
    public void Configure(EntityTypeBuilder<Buildings> builder)
    {
        builder.Property(a => a.BuildingName).IsRequired(false);
        builder.Property(a => a.CampusId).IsRequired();

        builder.HasOne(x => x.Campus)
            .WithMany(x => x.Buildings)
            .HasForeignKey(x => x.CampusId);
        builder.HasMany(x => x.Floors)
            .WithOne(x => x.Buildings)
            .HasForeignKey(x => x.BuildingId);
    }
}