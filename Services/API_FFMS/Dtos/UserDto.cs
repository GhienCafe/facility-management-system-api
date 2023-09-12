using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class UserDto : BaseDto
{
    public Guid? DepartmentId { get; set; }
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public string Email { get; set; } = null!;
}

public class UserUpdateDto
{
    public Guid? DepartmentId { get; set; }
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    public  UserStatus Status { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public DateTime? Dob { get; set; }
}

public class UserCreateDto
{
    public Guid? DepartmentId { get; set; }
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public DateTime? Dob { get; set; }
}

public class UserDetailDto :BaseDto{
    public Guid? DepartmentId { get; set; }
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
    public string Password { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public DateTime? FirstLoginAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class UserQueryDto : BaseQueryDto
{
    public string? UserCode { get; set; } = null;
    public string? Fullname { get; set; } = null;
    public string? Email { get; } = null;
}