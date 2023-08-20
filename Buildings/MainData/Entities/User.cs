﻿using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class User : BaseEntity
{
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    public  UserStatus Status { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Salt { get; set; } = null!;

    public DateTime? FirstLoginAt { get; set; }

    public DateTime? LastLoginAt { get; set; }
}

public enum UserRole
{
    GlobalAdmin = 1, Manager = 2, HRManager = 3, AssetManager = 4 
}


public enum UserStatus
{
    Active = 1, InActive = 2
}

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
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
    }
}
