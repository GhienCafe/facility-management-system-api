using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MainData.Entities;

public class Maintenance : BaseRequest
{
}

public class MaintenanceConfig : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.ToTable("Maintenances");
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.Notes).IsRequired(false);

        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Maintenances)
            .HasForeignKey(x => x.AssignedTo);
    }
}
