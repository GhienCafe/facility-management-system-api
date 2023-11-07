using System.ComponentModel.DataAnnotations;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class User : BaseEntity
{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
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
    public virtual IEnumerable<Token>? Tokens { get; set; }
    public virtual IEnumerable<TeamMember>? Teams { get; set; }
    public virtual IEnumerable<Notification>? Notifications { get; set; }
    
    public virtual IEnumerable<AssetCheck>? AssetChecks { get; set; }
    public virtual IEnumerable<Replacement>? Replacements { get; set; }
    public virtual IEnumerable<Repairation>? Repairations { get; set; }
    public virtual IEnumerable<Maintenance>? Maintenances { get; set; }
    public virtual IEnumerable<Transportation>? Transportations { get; set; }
    public virtual IEnumerable<InventoryCheck>? InventoryChecks { get; set; }
}

public enum UserRole
{
    [Display(Name = "Quản trị viên")]
    Administrator = 1,
    [Display(Name = "Quản lý")]
    Manager = 2,
    [Display(Name = "Nhân sự")]
    Staff = 3
}


public enum UserStatus
{
    [Display(Name = "Hoạt động")]
    Active = 1, 
    [Display(Name = "Không hoạt động")]
    InActive = 2
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
        builder.Property(x => x.UserCode).IsRequired();
        builder.Property(x => x.Dob).IsRequired(false);
        builder.Property(x => x.Gender).IsRequired();
        builder.Property(x => x.PersonalIdentifyNumber).IsRequired(false);

        // Attributes
        builder.HasIndex(x => x.UserCode).IsUnique();

        //Relationship
        builder.HasMany(x => x.Tokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        
        builder.HasMany(x => x.Teams)
            .WithOne(x => x.Member)
            .HasForeignKey(x => x.MemberId);
        
        // builder.HasMany(x => x.Requests)
        //     .WithOne(x => x.PersonInCharge)
        //     .HasForeignKey(x => x.AssignedTo);
        
        builder.HasMany(x => x.Notifications)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);

        builder.HasMany(x => x.AssetChecks)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasMany(x => x.Maintenances)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasMany(x => x.Replacements)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasMany(x => x.Repairations)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasMany(x => x.Transportations)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.AssignedTo);
        
        builder.HasMany(x => x.InventoryChecks)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.AssignedTo);
    }
}
