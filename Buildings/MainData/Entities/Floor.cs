using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Floor :BaseEntity
{
    public string? FloorMap { get; set; }
    public int FloorNumber { get; set; }
    public Guid BuildingId { get; set; }
    
    //Relationship
    public virtual IEnumerable<Room>? Rooms {get;set;}
    public virtual Building? Buildings { get; set; }
}

public class FloorConfig : IEntityTypeConfiguration<Floor>
{
    public void Configure(EntityTypeBuilder<Floor> builder)
    {
        builder.ToTable("Floors");
        builder.Property(a => a.FloorMap).IsRequired(false);
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