using Microsoft.Extensions.Configuration;

namespace AppCore.Extensions;

public static class EnvironmentExtension
{
    private static IConfiguration Configuration { get; }

    static EnvironmentExtension()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        Configuration = configurationBuilder.Build();
    }
    
    // ==================================== GLOBAL ======================================
    public static string GetAppLogFolder() =>
        Environment.GetEnvironmentVariable("LOG_FOLDER") ?? "Logs";

    public static string GetEnvironment() =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    public static bool IsSendOtp() => Environment.GetEnvironmentVariable("IS_SEND_OTP") == "True";
    public static bool IsProduction() => GetEnvironment() == "Production";
    public static bool IsStaging() => GetEnvironment() == "Staging";
    public static bool IsDevelopment() => GetEnvironment() == "Development";

    public static string GetAppConnectionString() =>
        Configuration["ENV_ENVIRONMENTS:CONNECTION_STRING"] ?? string.Empty;
    
    public static string GetPath() =>
        Configuration["ENV_ENVIRONMENTS:DOMAIN_PATH"] ?? string.Empty;
    
    public static string GetDomain() =>
        Configuration["ENV_ENVIRONMENTS:DOMAIN"] ?? string.Empty;

    public static string GetJwtIssuer() =>
        Configuration["ENV_ENVIRONMENTS:JWT_ISSUER"] ?? string.Empty;

    public static string GetJwtAudience() =>
        Configuration["ENV_ENVIRONMENTS:JWT_AUDIENCE"] ?? string.Empty;

    public static string GetJwtAccessTokenSecret() =>
        Configuration["ENV_ENVIRONMENTS:JWT_ACCESS_TOKEN_SECRET"] ?? string.Empty;

    public static double GetJwtAccessTokenExpires() =>
        Convert.ToDouble(Configuration["ENV_ENVIRONMENTS:JWT_ACCESS_TOKEN_EXPIRES"] ?? string.Empty);

    public static string GetJwtResetTokenSecret() =>
        Configuration["ENV_ENVIRONMENTS:JWT_RESET_TOKEN_SECRET"] ?? string.Empty;

    public static double GetJwtResetTokenExpires() =>
        Convert.ToDouble(Configuration["ENV_ENVIRONMENTS:JWT_RESET_TOKEN_EXPIRES"] ?? string.Empty);

    // public static string GetS3AccessKey() =>
    //     Environment.GetEnvironmentVariable("S3_ACCESS_KEY") ?? string.Empty;
    //
    // public static string GetS3SecretKey() =>
    //     Environment.GetEnvironmentVariable("S3_SECRET_KEY") ?? string.Empty;
    //
    // public static string GetS3ServiceUrl() =>
    //     Environment.GetEnvironmentVariable("S3_SERVICE_URL") ?? string.Empty;
    //
    // public static string GetBucketName() =>
    //     Environment.GetEnvironmentVariable("S3_BUCKET_NAME") ?? string.Empty;
    //
    // public static string GetS3EndpointUrl() =>
    //     Environment.GetEnvironmentVariable("S3_ENDPOINT_URL") ?? string.Empty;
    //
    // public static string GetApnBundleId() =>
    //     Environment.GetEnvironmentVariable("APN_BUNDLE_ID") ?? string.Empty;
    //
    // public static string GetApnCertFilePath() =>
    //     Environment.GetEnvironmentVariable("APN_CERT_FILE_PATH") ?? string.Empty;
    //
    // public static string GetApnKeyId() =>
    //     Environment.GetEnvironmentVariable("APN_KEY_ID") ?? string.Empty;
    //
    // public static string GetApnTeamId() =>
    //     Environment.GetEnvironmentVariable("APN_TEAM_ID") ?? string.Empty;

    public static string GetFireBaseCertFilePath()
    {
        if (IsProduction())
            return Environment.GetEnvironmentVariable("FB_CERT_FILE_PATH_PRODUCTION");
        return Environment.GetEnvironmentVariable(
            IsStaging() ? "FB_CERT_FILE_PATH_STAGING" : "FB_CERT_FILE_PATH_DEVELOPMENT"
        ) ?? string.Empty;
    }
}