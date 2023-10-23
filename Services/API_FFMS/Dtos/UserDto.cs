using AppCore.Attributes;
using AppCore.Configs;
using AppCore.Models;
using MainData.Entities;
using Newtonsoft.Json;

namespace API_FFMS.Dtos;

public class UserDto : BaseDto
{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public EnumValue? RoleObj { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? Dob { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? FirstLoginAt { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? LastLoginAt { get; set; } 
}

public class UserBaseDto : BaseDto
{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public UserRole? Role { get; set; }
    public EnumValue? RoleObj { get; set; }
    public string? Avatar { get; set; }
    public UserStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime? Dob { get; set; }
}

public class UserUpdateDto
{
    public string? UserCode { get; set; }
    public string? Fullname { get; set; }
    public UserRole? Role { get; set; }
    public string? Avatar { get; set; }
    public UserStatus? Status { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool? Gender { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    public DateTime? Dob { get; set; }
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
}

public class UserDetailDto :BaseDto{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public EnumValue? RoleObj { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool Gender { get; set; }
    public string? PersonalIdentifyNumber { get; set; }
    public DateTime? Dob { get; set; }
    public DateTime? FirstLoginAt { get; set; }
    public DateTime? LastLoginAt { get; set; } 
}

public class UserQueryDto : BaseQueryDto
{
    public UserRole? Role { get; set; }
    public bool? Gender { get; set; }
    public  UserStatus? Status { get; set; }
     
}