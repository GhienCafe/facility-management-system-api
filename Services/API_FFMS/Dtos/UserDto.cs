using AppCore.Attributes;
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class UserDto : BaseDto
{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public string Email { get; set; } = null!;
    public string? PersonalIdentifyNumber { get; set; }
}

public class UserUpdateDto
{
    public string? UserCode { get; set; }
    public string? Fullname { get; set; }
    public UserRole? Role { get; set; }
    public string? Avatar { get; set; }
    public  UserStatus? Status { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool? Gender { get; set; }
    public DateTime? Dob { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
}

public class UserCreateDto
{
    [Required] public string? UserCode { get; set; }
    [Required] public string Fullname { get; set; } = null!;
    [Required] public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    [Required] public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    public string? Address { get; set; }
    [Required] public bool Gender { get; set; }
    public DateTime? Dob { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
}

public class UserDetailDto :BaseDto{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    public  UserStatus Status { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public DateTime? Dob { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    public DateTime? FirstLoginAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class UserQueryDto : BaseQueryDto
{
    public string? UserCode { get; set; } = null;
    public string? Fullname { get; set; } = null;
    public string? Email { get; } = null;
}