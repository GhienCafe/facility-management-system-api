using Worker_Notify.Services;

namespace Worker_Notify.Workers
{
    public class InventoryWorker : BackgroundService
    {
        private readonly ILogger<InventoryWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeZoneInfo _localTimeZone = TimeZoneInfo.Local;

        private Timer _timer;

        public InventoryWorker(ILogger<InventoryWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, GetDelay(), TimeSpan.FromHours(24));

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private TimeSpan GetDelay()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _localTimeZone);
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

            if (scheduledTime <= now)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }
            TimeSpan delay = scheduledTime - now;
            return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
        }

        private void DoWork(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var inventoryConfigService = scope.ServiceProvider.GetRequiredService<IInventoryConfigService>();
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _localTimeZone);
                inventoryConfigService.CreateSystemNotification();
                _logger.LogInformation("Worker running at: {time}", localTime);
                _timer.Change(GetDelay(), TimeSpan.FromHours(24));
            }
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
