using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MainData.Entities;

public class Maintenance : BaseRequest
{
    public Guid AssetId { get; set; }
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
}

public class MaintenanceConfig : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.ToTable("Maintenances");
        builder.Property(x => x.Notes).IsRequired(false);
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(a => a.CategoryId).IsRequired(false);
        builder.Property(a => a.AssetTypeId).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Maintenances)
            .HasForeignKey(x => x.AssignedTo);
    }
}
