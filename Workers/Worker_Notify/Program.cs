using System.Reflection;
using AppCore.Configs;
using AppCore.Extensions;
using MainData;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using Worker_Notify;
using Worker_Notify.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, b =>
            {
                b.CommandTimeout(1200);
            });
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableDetailedErrors();
        });
        
        services.AddHttpContextAccessor();
        services.AddScoped<IMapperRepository, MapperRepository>();
        services.AddScoped<MainUnitOfWork>();

        services.AddScoped<IPushNotificationService, PushNotificationService>();
        services.AddScoped<ISendNotification, SendNotification>();
        
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();