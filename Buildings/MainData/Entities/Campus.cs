using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Campus: BaseEntity
{
    public string? CampusName { get; set; }
    public string? Telephone { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    
    //Relationship
    public virtual IEnumerable<Buildings>? Buildings { get; set; }
}

public class CampusConfig : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        builder.Property(a => a.CampusName).IsRequired(false);
        builder.Property(a => a.Telephone).IsRequired();
        builder.Property(a => a.Description).IsRequired();
        builder.Property(a => a.Address).IsRequired();
        
        //Relationship
        builder.HasMany(x => x.Buildings)
            .WithOne(x => x.Campus)
            .HasForeignKey(x => x.CampusId);
    }
}   
