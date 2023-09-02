using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class RequestDetail : BaseEntity
{
    public Guid RequestId { get; set; }
    public Guid CategoryId { get; set; }
    public string AssetName { get; set; } = null!;
    public double Quantity { get; set; }
    
    //
    public virtual AssetRequest? AssetRequest { get; set; }
    public virtual AssetCategory? Category { get; set; }
}

public class RequestDetailConfig : IEntityTypeConfiguration<RequestDetail>
{
    public void Configure(EntityTypeBuilder<RequestDetail> builder)
    {
        builder.ToTable("RequestDetails");
        builder.Property(a => a.RequestId).IsRequired();
        builder.Property(a => a.CategoryId).IsRequired();
        builder.Property(a => a.AssetName).IsRequired();
        builder.Property(a => a.Quantity).IsRequired();

        //Relationship
        builder.HasOne(x => x.Category)
            .WithMany(x => x.RequestDetails)
            .HasForeignKey(x => x.RequestId);
        
        builder.HasOne(x => x.AssetRequest)
            .WithMany(x => x.RequestDetails)
            .HasForeignKey(x => x.RequestId);
    }
}   
