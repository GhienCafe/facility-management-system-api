using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Model : BaseEntity
{
    public string? ModelName { get; set; }
    public string? Description { get; set; }
    
    //
    public IEnumerable<Asset>? Assets { get; set; }
}

public class ModelConfig : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.ToTable("Models");

        builder.Property(a => a.ModelName).IsRequired();
        builder.Property(a => a.Description).IsRequired(false);

        //Relationship
        builder.HasMany(x => x.Assets)
            .WithOne(x => x.Model)
            .HasForeignKey(x => x.ModelId);
    }
}   