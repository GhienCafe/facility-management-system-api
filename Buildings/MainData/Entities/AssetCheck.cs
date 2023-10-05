using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class AssetCheck : BaseRequest
{
    public bool IsVerified { get; set; } 
}

public class AssetCheckConfig : IEntityTypeConfiguration<AssetCheck>
{
    public void Configure(EntityTypeBuilder<AssetCheck> builder)
    {
        builder.ToTable("AssetChecks");
        builder.Property(x => x.IsVerified).IsRequired().HasDefaultValue(false);

        //Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.AssetChecks)
            .HasForeignKey(x => x.AssignedTo);
    }
}


