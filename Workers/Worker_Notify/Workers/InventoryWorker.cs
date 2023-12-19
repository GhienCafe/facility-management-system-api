using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Worker_Notify.Services;

namespace Worker_Notify.Workers
{
    public class InventoryWorker : BackgroundService
    {
        private readonly ILogger<InventoryWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeZoneInfo _localTimeZone = TimeZoneInfo.Local;

        private Timer _timer;

        public InventoryWorker(ILogger<InventoryWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait until 6:00 AM for the first execution
            var delay = CalculateDelayUntilNextExecution();
            _timer = new Timer(async _ =>
            {
                await DoWork();
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }, null, delay, TimeSpan.FromHours(24)); // Schedule to run every 24 hours

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task DoWork()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var appNotificationService = scope.ServiceProvider.GetRequiredService<IShortTermNotificationService>();
                await appNotificationService.CreateSystemNotification();
            }
        }

        private TimeSpan CalculateDelayUntilNextExecution()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _localTimeZone);
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 06, 00, 0); // Scheduled time at 6 AM

            if (now > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1); // If it's past 6 AM today, schedule for 6 AM tomorrow
            }

            return scheduledTime - now;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
