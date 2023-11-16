using System.Reflection;
using AppCore.Configs;
using AppCore.Data.Hubs;
using AppCore.Extensions;
using MainData;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using AspNetCore.Firebase.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

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
app.MapHub<NotificationHub>("chat-hub");

app.UseMiddleware<AuthMiddleware>();
app.Run();