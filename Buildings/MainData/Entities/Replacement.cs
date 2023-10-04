using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Replacement : BaseRequest
{
    public Guid NewAssetId { get; set; }
}

public class ReplacementConfig : IEntityTypeConfiguration<Replacement>
{
    public void Configure(EntityTypeBuilder<Replacement> builder)
    {
        builder.ToTable("Replacements");
        builder.Property(x => x.NewAssetId).IsRequired();
        
        // Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.Replacements)
            .HasForeignKey(x => x.AssignedTo);
    }
}
