using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Repairation : BaseRequest
{
}

public class RepairationConfig : IEntityTypeConfiguration<Repairation>
{
    public void Configure(EntityTypeBuilder<Repairation> builder)
    {
        builder.ToTable("Repairations");
        
        // Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Repairations)
            .HasForeignKey(x => x.AssignedTo);
    }
}