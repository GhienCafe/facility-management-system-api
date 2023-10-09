using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MainData.Entities;

public class RoomStatus : BaseEntity
{
    public string StatusName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Color { get; set; }
    
    //
    public virtual IEnumerable<Room>? Rooms { get; set; }
}

public class RoomStatusConfig : IEntityTypeConfiguration<RoomStatus>
{
    public void Configure(EntityTypeBuilder<RoomStatus> builder)
    {
        builder.ToTable("RoomStatus");
        builder.Property(x => x.StatusName).IsRequired();
        builder.Property(x => x.Description).IsRequired(false);
        builder.Property(x => x.Color).IsRequired();
        
        //Relationship
        builder.HasMany(x => x.Rooms)
            .WithOne(x => x.Status)
            .HasForeignKey(x => x.StatusId);
    }
}