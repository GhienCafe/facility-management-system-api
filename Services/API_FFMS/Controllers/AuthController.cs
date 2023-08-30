using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using MainData.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation("Login api")]
    public async Task<ApiResponse<AuthDto>> SignIn(AccountCredentialLoginDto accountCredentialLoginDto)
    {
        return await _authService.SignIn(accountCredentialLoginDto);
    }
    [HttpPost("Token-sign")]
    [AllowAnonymous]
    [SwaggerOperation("Login api")]
    public async Task<ApiResponse<AuthDto>> SignInToken(AuthTokenDto tokenDto)
    {
        return await _authService.Token(tokenDto);
    }

    [HttpPost("sign-out")]
    public async Task<ApiResponse> Logout()
    {
        return await _authService.RevokeToken();
    }
    
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [SwaggerOperation("Refresh token")]
    public async Task<ApiResponse<AuthDto>> SignIn(AuthRefreshDto authRefreshDto)
    {
        return await _authService.RefreshToken(authRefreshDto);
    }
}