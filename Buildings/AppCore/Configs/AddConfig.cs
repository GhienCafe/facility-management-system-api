using System.Net.Mime;
using System.Text.Json.Serialization;
using AppCore.Extensions;
using AppCore.Middlewares;
using AppCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using AspNetCore.Firebase.Authentication.Extensions;

namespace AppCore.Configs;

public static class AddConfigServiceCollectionExtensions
{
    private const string MyAllowAllOrigins = "_myAllowAllOrigins";

    public static void AddConfig(this IServiceCollection services, List<string> projectRegis,
        List<string> ignoreServices)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(MyAllowAllOrigins, policyBuilder =>
            {
                policyBuilder.WithOrigins(
                        "http://localhost",
                        "https://localhost",
                        "http://127.0.0.1:5500",
                        "https://127.0.0.1:5500",
                        "http://facility-management-system-fb.web.app",
                        "https://facility-management-system-fb.web.app")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("location", "Content-Disposition", "Link", "X-Total-Count", "X-Limit");
            });
        });

        services.AddConfigSwagger();
        services.AddHttpContextAccessor();

        // Config firebase
        services.AddFirebaseAuthentication(EnvironmentExtension.GetFirebaseIssuer(),
            EnvironmentExtension.GetFirebaseAudience());

        // Service regis service
        services.RegisAllService(projectRegis.ToArray(), ignoreServices.ToArray());

        //Configure the connection to Redis
        var redisConfiguration = ConfigurationOptions.Parse(EnvironmentExtension.GetRedisCachingServer());
        redisConfiguration.Password = EnvironmentExtension.GetRedisServePassword();

        // Register Redis ConnectionMultiplexer as a Singleton
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));

        // Add the Redis distributed cache service
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfiguration.ToString();
        });

        services.AddTransient<IDatabase>(provider =>
        {
            var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return connectionMultiplexer.GetDatabase();
        });

        // Config SignalR
        services.AddSignalR();

        // Service Other
        services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            options.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
        }).ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var result = new ValidationFailedResult(context.ModelState);
                result.ContentTypes.Add(MediaTypeNames.Application.Json);
                return result;
            };
        }).AddJsonOptions(option =>
        {
            option.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
            option.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            option.JsonSerializerOptions.Converters.Add(new LocalTimeZoneDateTimeConverter());
        });
        services.AddLogging(EnvironmentExtension.GetAppLogFolder());
    }

    public static void UseConfig(this IApplicationBuilder app)
    {
        app.Use((context, next) =>
        {
            context.Request.PathBase = EnvironmentExtension.GetPath();
            return next();
        });

        app.UseWebSockets();

        app.UseCors(MyAllowAllOrigins);

        if (EnvironmentExtension.GetEnvironment() == "Development")
        {
            app.UseConfigSwagger();
        }
        app.UseAuthentication();
        app.UseMiddleware<HandleResponseMiddleware>();
    }
}