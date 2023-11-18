using Microsoft.Extensions.Configuration;

namespace AppCore.Extensions;

public static class EnvironmentExtension
{
    // private static IConfiguration Configuration { get; }
    //
    // static EnvironmentExtension()
    // {
    //     var configurationBuilder = new ConfigurationBuilder()
    //         .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    //         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    //
    //     Configuration = configurationBuilder.Build();
    // }
    
    // ==================================== GLOBAL ======================================
    public static string GetAppLogFolder() =>
        Environment.GetEnvironmentVariable("LOG_FOLDER") ?? "Logs";

    public static string GetEnvironment() =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    public static bool IsSendOtp() => Environment.GetEnvironmentVariable("IS_SEND_OTP") == "True";
    public static bool IsProduction() => GetEnvironment() == "Production";
    public static bool IsStaging() => GetEnvironment() == "Staging";
    public static bool IsDevelopment() => GetEnvironment() == "Development";

    public static string GetTypeFirebase() => Environment.GetEnvironmentVariable("service_account") ?? string.Empty;
    public static string GetProjectIdFirebase() => Environment.GetEnvironmentVariable("PROJECT_ID") ?? string.Empty;
    public static string GetPrivateKeyIdFirebase() => Environment.GetEnvironmentVariable("PRIVATE_KEY_ID") ?? string.Empty;
    public static string GetPrivateKeyFirebase() => Environment.GetEnvironmentVariable("PRIVATE_KEY") ?? string.Empty;
    public static string GetClientEmailFireBase() => Environment.GetEnvironmentVariable("CLIENT_EMAIL") ?? string.Empty;
    public static string GetAccountEmailSystem() => Environment.GetEnvironmentVariable("ACCOUNT_EMAIL_SYSTEM") ?? string.Empty;
    public static string GetPasswordEmailPrivate() => Environment.GetEnvironmentVariable("PASSWORD_EMAIL_PRIVATE") ?? string.Empty;
    
    public static string GetAppConnectionString() =>
        Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? string.Empty;
    
    public static string GetRedisCachingServer() =>
        Environment.GetEnvironmentVariable("REDIS_CACHING_SERVER") ?? string.Empty;
    
    public static string GetRedisServePassword() =>
        Environment.GetEnvironmentVariable("REDIS_PWD") ?? string.Empty;
    
    public static string GetPath() =>
        Environment.GetEnvironmentVariable("DOMAIN_PATH") ?? string.Empty;
    
    public static string GetDomain() =>
        Environment.GetEnvironmentVariable("DOMAIN") ?? string.Empty;

    public static string GetJwtIssuer() =>
        Environment.GetEnvironmentVariable("JWT_ISSUER") ?? string.Empty;

    public static string GetJwtAudience() =>
        Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? string.Empty;

    public static string GetJwtAccessTokenSecret() =>
        Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_SECRET") ?? string.Empty;

    public static double GetJwtAccessTokenExpires() =>
        Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRES") ?? string.Empty);

    public static string GetJwtResetTokenSecret() =>
        Environment.GetEnvironmentVariable("JWT_RESET_TOKEN_SECRET") ?? string.Empty;

    public static double GetJwtResetTokenExpires() =>
        Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_RESET_TOKEN_EXPIRES") ?? string.Empty);

     public static string GetFirebaseIssuer() =>
            Environment.GetEnvironmentVariable("FIREBASE_ISSUER") ?? string.Empty;
            
     public static string GetFirebaseAudience() =>
            Environment.GetEnvironmentVariable("FIREBASE_AUDIENCE") ?? string.Empty;
    
    public static string GetFireBaseCertFilePath()
    {
        if (IsProduction())
            return Environment.GetEnvironmentVariable("FB_CERT_FILE_PATH_PRODUCTION");
        return Environment.GetEnvironmentVariable(
            IsStaging() ? "FB_CERT_FILE_PATH_STAGING" : "FB_CERT_FILE_PATH_DEVELOPMENT"
        ) ?? string.Empty;
    }
}