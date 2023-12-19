using AppCore.Extensions;
using MainData;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using Worker_Notify;
using Worker_Notify.Services;
using Worker_Notify.Workers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            var connectionString = EnvironmentExtension.GetAppConnectionString();
            options.UseSqlServer(connectionString, b =>
            {
                b.CommandTimeout(1200);
                //  b.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
            });
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableDetailedErrors();
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IMapperRepository, MapperRepository>();
        services.AddScoped<MainUnitOfWork>();

        services.AddScoped<IShortTermNotificationService, ShortTermNotificationService>();
        services.AddScoped<ISendNotification, SendNotification>();

        // Worker for push notification
        services.AddHostedService<Worker>();
        // Worker for create notification
        services.AddHostedService<InventoryWorker>();
    })
    .Build();

host.Run();