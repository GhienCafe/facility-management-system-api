using System.Reflection;
using AppCore.Configs;
using AppCore.Extensions;
using MainData;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

//Add DbContext
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(EnvironmentExtension.GetAppConnectionString(),
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
app.UseConfig();
//
app.MapControllers();

app.UseMiddleware<AuthMiddleware>();
app.Run();