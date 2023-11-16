using System.Reflection;
using AppCore.Configs;
using AppCore.Extensions;
using MainData;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using AspNetCore.Firebase.Authentication.Extensions;
using AppCore.Data.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Clear existing providers and add Console Logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Get the ILoggerFactory from the services
var loggerFactory = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>();

// Create a logger
var logger = loggerFactory.CreateLogger<Program>();

// Log a message when the application starts
logger.LogInformation($"The connection String is: {EnvironmentExtension.GetJwtAudience()}" );

//Add DbContext
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectionString = EnvironmentExtension.GetAppConnectionString();
    options.UseSqlServer(connectionString,
        b =>
        {
            b.CommandTimeout(1200);
        }
    );
    options.ConfigureWarnings(config =>
        {
            config.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
            config.Ignore(RelationalEventId.BoolWithDefaultWarning);
        }
    );
}, ServiceLifetime.Transient);

//
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddFirebaseAuthentication("https://securetoken.google.com/facility-management-system-fb", "facility-management-system-fb");
builder.Services.AddScoped<MainUnitOfWork>();
builder.Services.AddConfig(new List<string>
{
    "AppCore",
    "MainData",
    Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty
}, new List<string>
{
    nameof(IApiVersionDescriptionProvider),
    "UnitOfWork",
    "IUnitOfWork"
});

//

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseConfig();
//
app.MapControllers();
app.MapHub<TestHub>("test-hub");

app.UseMiddleware<AuthMiddleware>();
app.Run();