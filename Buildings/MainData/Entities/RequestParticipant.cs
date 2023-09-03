using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class RequestParticipant : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RequestId { get; set; }
    public RequestRole Role { get; set; }
    
    //
    public virtual User? User { get; set; }
    public virtual AssetRequest? AssetRequest { get; set; }
}

public enum RequestRole
{
    Requestor = 1,
    Approver = 2,
    Responsible = 3
}

public class RequestParticipantConfig : IEntityTypeConfiguration<RequestParticipant>
{
    public void Configure(EntityTypeBuilder<RequestParticipant> builder)
    {
        builder.ToTable("RequestParticipants");
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.RequestId).IsRequired();
        builder.Property(a => a.Role).IsRequired();

        //Relationship
        builder.HasOne(x => x.AssetRequest)
            .WithMany(x => x.RequestParticipants)
            .HasForeignKey(x => x.RequestId);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.RequestParticipants)
            .HasForeignKey(x => x.RequestId);
    }
}   