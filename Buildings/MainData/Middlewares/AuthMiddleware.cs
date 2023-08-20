using System.Security.Claims;
using AppCore.Extensions;
using AppCore.Models;
using MainData.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MainData.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext httpContext,
        DatabaseContext context)
    {
        var accessToken = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var tokenClaim = JwtExtensions.ValidateAccessToken(accessToken ?? string.Empty).ToList();
        var accountIdString = tokenClaim.Find(x => x.Type == AppClaimTypes.AccountId)?.Value;
        Guid.TryParse(accountIdString, out var accountId);
        if (accountId != Guid.Empty)
        {
            var now = DatetimeExtension.UtcNow();
            var account = await context.Users.FirstOrDefaultAsync(a =>
                !a.DeletedAt.HasValue &&
                a.Id == accountId
            );
            var token = await context.Tokens
                .FirstOrDefaultAsync(t =>
                    !t.DeletedAt.HasValue &&
                    t.AccessToken == accessToken &&
                    t.UserId == accountId &&
                    t.AccessExpiredAt > now &&
                    t.Status == TokenStatus.Active
                );

            if (account == null || token == null)
            {
                await _next(httpContext);
                return;
            }

            if (account.Role is UserRole.GlobalAdmin)
            {
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(AppClaimTypes.AccountId, account.Id.ToString()),
                    new Claim(AppClaimTypes.Role, account.Role.ToString()),
                    new Claim(AppClaimTypes.Status, account.Status.ToString()),
                }));
                await _next(httpContext);
                return;
            }

            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(AppClaimTypes.AccountId, account.Id.ToString()),
                new Claim(AppClaimTypes.Role, account.Role.ToString()),
                new Claim(AppClaimTypes.Status, account.Status.ToString()),
            }));
            await _next(httpContext);
            return;
        }

        await _next(httpContext);
    }
}