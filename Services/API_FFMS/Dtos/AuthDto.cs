using AppCore.Attributes;
using AppCore.Configs;
using MainData.Entities;
using Newtonsoft.Json;

namespace API_FFMS.Dtos;

public class AccountCredentialLoginDto
{
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    [Required] public string OldPassword { get; set; } = null!;
    [Required] public string NewPassword { get; set; } = null!;
}

public class AuthDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime AccessExpiredAt { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime RefreshExpiredAt { get; set; }

    public string? Email { get; set; }
    public string? Fullname { get; set; }
    public UserRole Role { get; set; }
    public Guid UserId { get; set; }
    public bool IsFirstLogin { get; set; }
}

public class ResetPasswordDto
{
    [Required] public string Email { get; set; } = string.Empty;
}
public class UpdatePasswordDto
{
    [Required] public string Email { get; set; } = string.Empty;

    [Required] public string ResetCode { get; set; } = string.Empty;
    [Required] public string NewPassword { get; set; } = string.Empty;
}

public class AuthRefreshDto
{
    [Required] public string RefreshToken { get; set; } = string.Empty;
}
public class AuthTokenDto
{
    [Required] public string AccessToken { get; set; } = string.Empty;
}