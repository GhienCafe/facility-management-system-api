using System;
using System.Threading;
using Worker_Notify.Services;

namespace Worker_Notify
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var appNotificationService = scope.ServiceProvider.GetRequiredService<IShortTermNotificationService>();
                    await Task.Run(() => appNotificationService.SendTask());
                    
                    var delay = TimeSpan.FromSeconds(1);
                    // var delay = TimeSpan.FromDays(30);
                    var webNotificationService = scope.ServiceProvider.GetRequiredService<IWebNotificationService>();
                    await Task.Run(() =>  webNotificationService.SendMaintenance(delay));
                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}