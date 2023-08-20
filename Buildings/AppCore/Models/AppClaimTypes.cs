using AppCore.Extensions;

namespace AppCore.Models;

public static class AppClaimTypes
{
    public static readonly string AccountId = $"{EnvironmentExtension.GetDomain()}/account_id";
    public static readonly string Role = $"{EnvironmentExtension.GetDomain()}/role";
    public static readonly string Status = $"{EnvironmentExtension.GetDomain()}/status";
    public static readonly string IsActive = $"{EnvironmentExtension.GetDomain()}/is_active";
    public static readonly string UserId = $"{EnvironmentExtension.GetDomain()}/user_id";
}