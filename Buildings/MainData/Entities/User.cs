using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class User : BaseEntity
{
    public Guid DepartmentId { get; set; }
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    public  UserStatus Status { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    public DateTime? Dob { get; set; }
    public string Password { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public DateTime? FirstLoginAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    //
    public virtual Department? Department { get; set; }
    public virtual IEnumerable<RequestParticipant>? RequestParticipants { get; set; }
    public virtual IEnumerable<HandoverParticipant>? HandoverParticipants { get; set; }
    public virtual IEnumerable<MaintenanceParticipant>? MaintenanceParticipants { get; set; }
    public virtual IEnumerable<InventoryTeamMember>? InventoryTeamMembers { get; set; }

}

public enum UserRole
{
    GlobalManager = 1,
    CampusManagers = 2,
    FacilitiesManager = 3,
    TechnicalSpecialist = 4
}


public enum UserStatus
{
    Active = 1, InActive = 2
}

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.Property(x => x.Fullname).IsRequired();
        builder.Property(x => x.Role).IsRequired();
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.Avatar).IsRequired(false);
        builder.Property(x => x.PhoneNumber).IsRequired();
        builder.Property(x => x.Address).IsRequired(false);
        builder.Property(x => x.Password).IsRequired();
        builder.Property(x => x.Salt).IsRequired();
        builder.Property(x => x.FirstLoginAt).IsRequired(false);
        builder.Property(x => x.LastLoginAt).IsRequired(false);
        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.UserCode).IsRequired();
        builder.Property(x => x.Dob).IsRequired(false);
        builder.Property(x => x.Gender).IsRequired();
        builder.Property(x => x.PersonalIdentifyNumber).IsRequired(false);
        
        // Attributes
        builder.HasIndex(x => x.UserCode).IsUnique();
        
        //Relationship
        builder.HasOne(x => x.Department)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.DepartmentId);
        
        builder.HasOne(x => x.Department)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.DepartmentId);
        
        builder.HasMany(x => x.RequestParticipants)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        
        builder.HasMany(x => x.HandoverParticipants)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        
        builder.HasMany(x => x.MaintenanceParticipants)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        
        builder.HasMany(x => x.InventoryTeamMembers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
