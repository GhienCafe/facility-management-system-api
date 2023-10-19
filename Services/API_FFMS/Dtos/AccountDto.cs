using AppCore.Configs;
using AppCore.Models;
using MainData.Entities;
using Newtonsoft.Json;

namespace API_FFMS.Dtos;

public class AccountDto : BaseDto
{
    public string UserCode { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
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

public class AccountUpdateDto
{
    public string Fullname { get; set; } = null!;
    public string? Avatar { get; set; }
    public string Address { get; set; } = null!;
    public DateTime? Dob { get; set; }
}
public class AccountQueryDto : BaseQueryDto
{
    public string? Keyword { get; set; }
}
