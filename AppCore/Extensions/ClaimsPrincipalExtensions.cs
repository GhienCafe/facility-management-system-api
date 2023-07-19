using System.Security.Claims;
using AppCore.Models;

namespace AppCore.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetAccountId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(AppClaimTypes.AccountId);
            Guid.TryParse(value, out var accountId);
            return accountId;
        }

        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(AppClaimTypes.UserId);
            Guid.TryParse(value, out var userId);
            return userId;
        }

        public static bool GetActive(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(AppClaimTypes.IsActive);
            return bool.TryParse(value, out var isActive) && isActive;
        }

        public static string GetRole(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(AppClaimTypes.Role);
        }

        public static TEnum? GetEnum<TEnum>(this ClaimsPrincipal principal, string claimType) where TEnum : struct
        {
            var value = principal.FindFirstValue(claimType);
            return Enum.TryParse(value, out TEnum result) ? result : null;
        }
    }
}