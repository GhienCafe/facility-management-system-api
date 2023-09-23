using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class RoomType : BaseEntity
{
    public string TypeName { get; set; } = null!;
    public string? Description { get; set; }
    
    //
    public IEnumerable<Room>? Rooms { get; set; } 
}

public class RoomTypeConfig : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        builder.ToTable("RoomTypes");
        builder.Property(x => x.TypeName).IsRequired();
        builder.Property(x => x.Description).IsRequired(false);
        
        //Relationship
        builder.HasMany(x => x.Rooms)
            .WithOne(x => x.RoomType)
            .HasForeignKey(x => x.RoomTypeId);

    }
}
