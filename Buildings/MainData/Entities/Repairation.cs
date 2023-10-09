using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Repairation : BaseRequest
{
    public Guid AssetId { get; set; }
}

public class RepairationConfig : IEntityTypeConfiguration<Repairation>
{
    public void Configure(EntityTypeBuilder<Repairation> builder)
    {
        builder.ToTable("Repairations");
        builder.Property(x => x.AssetId).IsRequired();
        
        // Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Repairations)
            .HasForeignKey(x => x.AssignedTo);
    }
}