using AppCore.Attributes;
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class UserDto : BaseDto
{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public EnumValue? Role { get; set; }
    public string? Avatar { get; set; }
    public EnumValue? Status { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    public DateTime? Dob { get; set; }
    public DateTime? FirstLoginAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Guid? TeamId { get; set; }
    //public TeamDto? Team { get; set; }
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
    public Guid? TeamId { get; set; }
}

public class UserCreateDto
{
    [Required(ErrorMessage = "Mã người dùng không được trống")]
    public string UserCode { get; set; } = null!;
    [Required(ErrorMessage = "Tên người dùng không được trống")]
    public string Fullname { get; set; } = null!;
    [Required(ErrorMessage = "Vai trò người dùng không được trống")]
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    [Required(ErrorMessage = "Email dùng không được trống")]
    [Email(ErrorMessage = "Không đúng định dạng email")]
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    [Required(ErrorMessage = "Giới tính người dùng không được trống")]
    public bool Gender { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    public DateTime? Dob { get; set; }
    //public string Password { get; set; } = null!;
    public Guid? TeamId { get; set; }
}

public class UserDetailDto :BaseDto{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public EnumValue? Role { get; set; }
    public string? Avatar { get; set; }
    public EnumValue? Status { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    public DateTime? Dob { get; set; }
    public DateTime? FirstLoginAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Guid? TeamId { get; set; }
    public TeamDto? Team { get; set; }
}

public class UserQueryDto : BaseQueryDto
{
    public UserRole? Role { get; set; }
    public bool? Gender { get; set; }
    public  UserStatus? Status { get; set; }
    public  Guid? TeamId { get; set; }
     
}