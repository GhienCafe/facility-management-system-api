using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Floors :BaseEntity
{
    public double? Area { get; set; }
    public string? PathFloor { get; set; }
    public string? FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
    
    //Relationship
    public virtual IEnumerable<Rooms>? Rooms {get;set;}
    public virtual Buildings? Buildings { get; set; }
}

public class FloorsConfig : IEntityTypeConfiguration<Floors>
{
    public void Configure(EntityTypeBuilder<Floors> builder)
    {
        builder.Property(a => a.Area).IsRequired(false);
        builder.Property(a => a.PathFloor).IsRequired();
        builder.Property(a => a.FloorNumber).IsRequired();
        builder.Property(a => a.BuildingId).IsRequired();
        //Relationship
        builder.HasOne(x => x.Buildings)
            .WithMany(x => x.Floors)
            .HasForeignKey(x => x.BuildingId);
        builder.HasMany(x => x.Rooms)
            .WithOne(x => x.Floors)
            .HasForeignKey(x => x.FloorId);
    }
}