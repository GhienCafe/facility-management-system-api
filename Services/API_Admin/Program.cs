using System.Reflection;
using AppCore.Configs;
using AppCore.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//
// builder.Services.AddDbContext<DatabaseContext>(options =>
// {
//     var connectString = EnvironmentExtension.GetAppConnectionString();
//     options.UseNpgsql(connectString,
//         b =>
//         {
//             b.MigrationsAssembly(Assembly.GetEntryAssembly()?.GetName().Name);
//             b.CommandTimeout(1200);
//         }
//     );
//     options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
//     options.EnableDetailedErrors();
// }, ServiceLifetime.Transient);

//
builder.Services.AddControllers();
//builder.Services.AddScoped<MainUnitOfWork>();
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
builder.Host.UseSerilog();
//

var app = builder.Build();
app.UseConfig(EnvironmentExtension.GetPath());
//
app.MapControllers();
// app.UseMiddleware<AuthMiddleware>();
app.Run();