using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class AssetRequest : BaseEntity
{
    public string RequestCode { get; set; } = null!;
    public string? Reason { get; set; }
    public string? Content { get; set; }
    public string? Note { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    
    //
    public virtual IEnumerable<RequestParticipant>? RequestParticipants { get; set; }
    public virtual IEnumerable<RequestDetail>? RequestDetails { get; set; }
}

public enum RequestStatus
{
    Created = 1,
    PendingApproval = 2,
    Approved = 3,
    InProgress = 4,
    Completed = 5,
    Rejected = 6,
    Expired = 7
}

public class AssetRequestConfig : IEntityTypeConfiguration<AssetRequest>
{
    public void Configure(EntityTypeBuilder<AssetRequest> builder)
    {
        builder.ToTable("AssetRequests");
        builder.Property(a => a.RequestCode).IsRequired();
        builder.Property(a => a.Reason).IsRequired(false);
        builder.Property(a => a.Content).IsRequired(false);
        builder.Property(a => a.Note).IsRequired(false);
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.RequestDate).IsRequired();
        builder.Property(a => a.ApprovedDate).IsRequired(false);
        
        // Attributes
        builder.HasIndex(a => a.RequestCode).IsUnique();

        //Relationship
        builder.HasMany(x => x.RequestParticipants)
            .WithOne(x => x.AssetRequest)
            .HasForeignKey(x => x.RequestId);
        
        builder.HasMany(x => x.RequestDetails)
            .WithOne(x => x.AssetRequest)
            .HasForeignKey(x => x.RequestId);
    }
}   
