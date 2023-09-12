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

namespace API_FFMS.Services;

public interface IAuthService : IBaseService
{
    Task<ApiResponse<AuthDto>> SignIn(AccountCredentialLoginDto accountCredentialLoginDto);
    Task<ApiResponse<AuthDto>> Token(AuthTokenDto authTokenDto);
    Task<ApiResponse<AuthDto>> RefreshToken(AuthRefreshDto authRefreshDto);
    Task<ApiResponse> RevokeToken();
}

public class AuthService : BaseService, IAuthService
{
    public AuthService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    public async Task<ApiResponse<AuthDto>> SignIn(AccountCredentialLoginDto accountCredentialLoginDto)
    {
        // Mẫu đúng cho định dạng email của FPT hoặc FE
        string fptOrFeEmailPattern = @"^[A-Za-z0-9._%+-]+@(fpt\.edu\.vn|fe\.edu\.vn)$";
        var user = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue && x.Email == accountCredentialLoginDto.Email
        });

        if (user == null)
        {
            throw new ApiException("Not existed user",StatusCode.NOT_FOUND);
        }
        // Kiểm tra xem email có khớp với mẫu FPT hoặc FE hay không
        if (!Regex.IsMatch(accountCredentialLoginDto.Email, fptOrFeEmailPattern))
        {
            throw new ApiException("Invalid FPT or FE email format", StatusCode.FORBIDDEN);
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
             throw new ApiException("Đang k biết lỗi gì", StatusCode.SERVER_ERROR);
        
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
    
    private IEnumerable<Claim> SetClaims(User account)
    {
        // Create token
        var claims = new Claim[]
        {
            new(AppClaimTypes.AccountId, account.Id.ToString()),
            new(AppClaimTypes.Role, account.Role.ToString()),
            new(AppClaimTypes.Status, account.Status.ToString()),
        }.ToList();
        return claims;
    }
}