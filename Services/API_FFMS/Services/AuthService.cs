using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.RegularExpressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface IAuthService : IBaseService
{
    Task<ApiResponse<AuthDto>> SignIn(AccountCredentialLoginDto accountCredentialLoginDto);
    Task<ApiResponse<AuthDto>> Token(AuthTokenDto authTokenDto);
    Task<ApiResponse<AuthDto>> RefreshToken(AuthRefreshDto authRefreshDto);
    Task<ApiResponse> RevokeToken();
    Task<ApiResponse> ResetPassword(UpdatePasswordDto resetPasswordDto);
    Task<ApiResponse> SendMailConfirmPassword(ResetPasswordDto email);
}

public class AuthService : BaseService, IAuthService
{
    public AuthService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    public async Task<ApiResponse<AuthDto>> SignIn(AccountCredentialLoginDto accountCredentialLoginDto)
    {
        var user = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue && x.Email == accountCredentialLoginDto.Email
        });

        if (user == null)
        {
            throw new ApiException("Not existed user",StatusCode.NOT_FOUND);
        }
        
        // Check status
        if (user.Status == UserStatus.InActive)
            throw new ApiException(MessageKey.AccountNotActivated, StatusCode.NOT_ACTIVE);
        
        // Check password
        if (!accountCredentialLoginDto.Password.VerifyPassword<User>(user.Salt!, user.Password!))
        {
            throw new ApiException("Incorrect password", StatusCode.BAD_REQUEST);
        }
        
        var claims = SetClaims(user);
        var minute = EnvironmentExtension.GetJwtAccessTokenExpires();
        var refreshMinute = EnvironmentExtension.GetJwtResetTokenExpires();
        var accessExpiredAt = CurrentDate.AddMinutes(minute);
        var refreshExpiredAt = CurrentDate.AddMinutes(refreshMinute);
        var accessToken = JwtExtensions.GenerateAccessToken(claims, accessExpiredAt);
        var refreshToken = JwtExtensions.GenerateRefreshToken();

        var token = new Token
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Type = TokenType.SignInToken,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessExpiredAt = accessExpiredAt,
            RefreshExpiredAt = refreshExpiredAt,
            Status = TokenStatus.Active
        };
        
         if (!await MainUnitOfWork.TokenRepository.InsertAsync(token, user.Id, CurrentDate))
             throw new ApiException("Token fail", StatusCode.SERVER_ERROR);
        
        var verifyResponse = new AuthDto
        {
            AccessExpiredAt = accessExpiredAt,
            RefreshExpiredAt = refreshExpiredAt,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email,
            Fullname = user.Fullname,
            Role = user.Role,
            UserId = user.Id,
            IsFirstLogin = (user.FirstLoginAt == null)
        };
        
        user.FirstLoginAt ??= CurrentDate;

        if (!await MainUnitOfWork.UserRepository.UpdateAsync(user, Guid.Empty, CurrentDate))
            throw new ApiException("Login fail!", StatusCode.SERVER_ERROR);
        
        return ApiResponse<AuthDto>.Success(verifyResponse);
    }
    public async Task<ApiResponse<AuthDto>> Token(AuthTokenDto authTokenDto)
    {
        // Mẫu đúng cho định dạng email của FPT hoặc FE
        string fptOrFeEmailPattern = @"^[A-Za-z0-9._%+-]+@(fpt\.edu\.vn|fe\.edu\.vn)$";
        var tokenInfo = new Dictionary<string, string>();
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(authTokenDto.AccessToken);
        var claimsToken = jwtSecurityToken.Claims.ToList();
        var verifyResponse = new AuthDto();
        foreach (var claim in claimsToken)
        {
            tokenInfo.Add(claim.Type, claim.Value);
        }
        var keys = tokenInfo.Keys;
        var values = tokenInfo.Values;
        User? user;
        foreach (var key in keys)
        {
            var value = tokenInfo[key];
            if (key.Trim().ToLower().Equals("email"))
            {
                // Ensure both the token email claim and the database email are in the same case
                user = MainUnitOfWork.UserRepository.GetQuery()
                    .Where(x => x.Email.Trim().ToLower() == value.Trim().ToLower()).SingleOrDefault();
                // Kiểm tra xem email có khớp với mẫu FPT hoặc FE hay không
                if (!Regex.IsMatch(value, fptOrFeEmailPattern))
                {
                    throw new ApiException("Invalid FPT or FE email format", StatusCode.UNAUTHORIZED);
                }

                if (user == null)
                {
                    throw new ApiException("Not existed user", StatusCode.NOT_FOUND);
                }

                // Check status
                if (user.Status == UserStatus.InActive)
                    throw new ApiException(MessageKey.AccountNotActivated, StatusCode.NOT_ACTIVE);

                var claims = SetClaims(user);
                var minute = EnvironmentExtension.GetJwtAccessTokenExpires();
                var refreshMinute = EnvironmentExtension.GetJwtResetTokenExpires();
                var accessExpiredAt = CurrentDate.AddMinutes(minute);
                var refreshExpiredAt = CurrentDate.AddMinutes(refreshMinute);
                var accessToken = JwtExtensions.GenerateAccessToken(claims, accessExpiredAt);
                var refreshToken = JwtExtensions.GenerateRefreshToken();

                var token = new Token
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Type = TokenType.SignInToken,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessExpiredAt = accessExpiredAt,
                    RefreshExpiredAt = refreshExpiredAt,
                    Status = TokenStatus.Active
                };

                if (!await MainUnitOfWork.TokenRepository.InsertAsync(token, user.Id, CurrentDate))
                    throw new ApiException("Đang k biết lỗi gì", StatusCode.SERVER_ERROR);

                verifyResponse = new AuthDto
                {
                    AccessExpiredAt = accessExpiredAt,
                    RefreshExpiredAt = refreshExpiredAt,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Role = user.Role,
                    UserId = user.Id,
                    IsFirstLogin = (user.FirstLoginAt == null)
                };

                user.FirstLoginAt ??= CurrentDate;

                if (!await MainUnitOfWork.UserRepository.UpdateAsync(user, Guid.Empty, CurrentDate))
                    throw new ApiException("Login fail!", StatusCode.SERVER_ERROR);

                return ApiResponse<AuthDto>.Success(verifyResponse);
            }
            
        }
        throw new ApiException("Login Fail", StatusCode.UNAUTHORIZED);
    }

    public async Task<ApiResponse<AuthDto>> RefreshToken(AuthRefreshDto authRefreshDto)
    {
        var token = await MainUnitOfWork.TokenRepository.FindOneAsync(new Expression<Func<Token, bool>>[]
        {
            t => t.RefreshToken == authRefreshDto.RefreshToken,
            t => t.Type == TokenType.SignInToken
        });

        if (token == null)
            throw new ApiException("Not found", StatusCode.NOT_FOUND);

        var account = await MainUnitOfWork.UserRepository.FindOneAsync(token.UserId);
        if (account != null && account.Status == UserStatus.InActive)
            throw new ApiException("Not found", StatusCode.NOT_FOUND);

        if (Math.Abs((token.AccessExpiredAt - CurrentDate).TotalMinutes) < 1)
            throw new ApiException("Token is still valid", StatusCode.BAD_REQUEST);

        var claims = SetClaims(account!);
        var accessExpiredAt = CurrentDate.AddMinutes(EnvironmentExtension.GetJwtAccessTokenExpires());
        var refreshExpiredAt = CurrentDate.AddMinutes(EnvironmentExtension.GetJwtResetTokenExpires());
        var accessToken = JwtExtensions.GenerateAccessToken(claims, accessExpiredAt);
        var refreshToken = JwtExtensions.GenerateRefreshToken();

        token.AccessToken = accessToken;
        token.RefreshToken = refreshToken;
        token.AccessExpiredAt = accessExpiredAt;
        token.RefreshExpiredAt = refreshExpiredAt;
        token.Status = TokenStatus.Active;

        if (!await MainUnitOfWork.TokenRepository.UpdateAsync(token, account!.Id, CurrentDate))
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);

        // Update current device token for push notify
        return ApiResponse<AuthDto>.Success(new AuthDto
        {
            AccessExpiredAt = accessExpiredAt,
            RefreshExpiredAt = refreshExpiredAt,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = account.Email,
            Fullname = account.Fullname,
            Role = account.Role,
            UserId = account.Id,
            IsFirstLogin = (account.FirstLoginAt == null)
        });
    }
    
    public async Task<ApiResponse> RevokeToken()
    {
        //Get token from header
        var bearToken = string.Empty;
        if (HttpContextAccessor.HttpContext != null)
        {
            bearToken = HttpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                ?.Split(" ").Last();
        }

        // Find token
        var token = await MainUnitOfWork.TokenRepository.FindOneAsync(new Expression<Func<Token, bool>>[]
        {
            x => x.AccessToken == bearToken,
            x => !x.DeletedAt.HasValue
        });

        //Find account 
        var account = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
        {
            x => x.Id == token!.UserId,
            x => !x.DeletedAt.HasValue
        });

        if (account == null || token == null)
            throw new ApiException("Token is incorrect", StatusCode.BAD_REQUEST);

        // Update - delete token
        token.AccessExpiredAt = CurrentDate;
        token.RefreshExpiredAt = CurrentDate;

        if (!(await MainUnitOfWork.TokenRepository.DeleteAsync(token, null, CurrentDate)))
        {
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
        }

        account.LastLoginAt = CurrentDate;
        // Update account
        if (!(await MainUnitOfWork.UserRepository.UpdateAsync(account, account.Id, CurrentDate)))
        {
            throw new ApiException(MessageKey.ServerError, StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }
    
    public async Task<ApiResponse> SendMailConfirmPassword(ResetPasswordDto email)
{
    var user =  MainUnitOfWork.UserRepository.GetQuery().SingleOrDefault(x => !x!.DeletedAt.HasValue && x.Email == email.Email);
    if (user == null)
    {
        throw new ApiException("Người dùng không tìm thấy", StatusCode.NOT_FOUND);
    }
    if (!user.Email.EndsWith("@fpt.edu.vn") && !user.Email.EndsWith("@fu.edu.vn"))
    {
        // Nếu không kết thúc bằng các giá trị cần kiểm tra
        throw new ApiException("Email không hợp lệ", StatusCode.BAD_REQUEST);
    }


    var resetCode = GenerateRandomCode(6);
    var claims = SetClaims(user);
    var accessExpiredAt = CurrentDate.AddMinutes(EnvironmentExtension.GetJwtAccessTokenExpires());

    var resetToken =  MainUnitOfWork.TokenRepository.GetQuery()
        .SingleOrDefault(x =>!x!.DeletedAt.HasValue && x.UserId == user.Id && x.Type == TokenType.ResetPassword);

    if (resetToken == null)
    {
        resetToken = new Token
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Type = TokenType.ResetPassword,
            AccessToken = JwtExtensions.GenerateAccessToken(claims, accessExpiredAt),
            RefreshToken = resetCode,
            AccessExpiredAt = DateTime.Now.AddMinutes(2),
            RefreshExpiredAt = DateTime.Now.AddMinutes(2),
            Status = TokenStatus.Active
        };

        if (!await MainUnitOfWork.TokenRepository.InsertAsync(resetToken, user.Id, CurrentDate))
        {
            throw new ApiException("Không thể gửi mã vui lòng thử lại sau", StatusCode.BAD_REQUEST);
        }
    }
    else
    {
        // Nếu token đã tồn tại, cập nhật lại các giá trị phù hợp
        resetToken.AccessToken = JwtExtensions.GenerateAccessToken(claims, accessExpiredAt);
        resetToken.RefreshToken = resetCode;
        resetToken.AccessExpiredAt = DateTime.Now.AddMinutes(2);
        resetToken.RefreshExpiredAt = DateTime.Now.AddMinutes(2);
        resetToken.Status = TokenStatus.Active;

        // Thử cập nhật token
        if (!await MainUnitOfWork.TokenRepository.UpdateAsync(resetToken, user.Id, CurrentDate))
        {
            throw new ApiException("Không thể gửi mã vui lòng thử lại sau", StatusCode.BAD_REQUEST);
        }
    }

    MailExtension mailExtension = new MailExtension();
    var resetEmailBody = $"Mã code của bạn là: {resetCode}";
    mailExtension.SendMailCommon("Cập nhật lại mật khẩu", user.Fullname, user.Email, resetEmailBody);

    return ApiResponse.Success();
}

  public async Task<ApiResponse> ResetPassword(UpdatePasswordDto resetPasswordDto)
  {
    var user = await MainUnitOfWork.UserRepository.GetQuery().SingleOrDefaultAsync(x => !x!.DeletedAt.HasValue && x.Email == resetPasswordDto.Email);
    if (user == null)
    {
      throw new ApiException("Không tìm thấy người dùng", StatusCode.NOT_FOUND);
    }

    var resetToken = await MainUnitOfWork.TokenRepository.GetQuery().SingleOrDefaultAsync(x => x!.UserId == user.Id && x.RefreshToken == resetPasswordDto.ResetCode);

    if (resetToken == null)
    {
      throw new ApiException("Mã không hợp lẹ", StatusCode.BAD_REQUEST);
    }

    if (resetToken.RefreshExpiredAt < DateTime.Now)
    {
      throw new ApiException("Mã xác nhận đã hết hạn, vui lòng yêu cầu lại mã mới", StatusCode.UNAUTHORIZED);
    }
    if (resetToken.AccessExpiredAt < DateTime.Now)
    {
        throw new ApiException("Mã xác nhận đã hết hạn, vui lòng yêu cầu lại mã mới", StatusCode.UNAUTHORIZED);
    }
    var salt = SecurityExtension.GenerateSalt();
    user.Password = SecurityExtension.HashPassword<User>(resetPasswordDto.NewPassword, salt);
    user.Salt = salt;
    if (!await MainUnitOfWork.UserRepository.UpdateAsync(user, AccountId, CurrentDate))
      throw new ApiException("Update Password fail", StatusCode.SERVER_ERROR);

    return ApiResponse.Success();
  }

  private string GenerateRandomCode(int length)
  {
    var random = new Random();
    var code = string.Empty;
    for (int i = 0; i < length; i++)
    {
      code += random.Next(0, 9).ToString();
    }
    return code;
  }
    
    private IEnumerable<Claim> SetClaims(User account)
    {
        // Create token
        var claims = new Claim[]
        {
            new(AppClaimTypes.UserId, account.Id.ToString()),
            new(AppClaimTypes.Role, account.Role.ToString()),
            new(AppClaimTypes.Status, account.Status.ToString()),
            new(AppClaimTypes.IsActive, (account.Status == UserStatus.Active).ToString()),
        }.ToList();
        return claims;
    }
}