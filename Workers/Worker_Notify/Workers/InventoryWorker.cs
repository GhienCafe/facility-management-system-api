using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Worker_Notify.Services;

namespace Worker_Notify.Workers
{
    public class InventoryWorker : BackgroundService
    {
        private readonly ILogger<InventoryWorker> _logger;
        private readonly IInventoryConfigService _inventoryConfigService;
        private readonly TimeZoneInfo _localTimeZone = TimeZoneInfo.Local;

        private Timer _timer;

        public InventoryWorker(ILogger<InventoryWorker> logger, IInventoryConfigService inventoryConfigService)
        {
            _logger = logger;
            _inventoryConfigService = inventoryConfigService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, GetDelay(), TimeSpan.FromHours(24)); // Khởi động timer với thời gian delay

            // Đợi cho sự kết thúc hoặc hủy bỏ của công việc
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private TimeSpan GetDelay()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _localTimeZone);
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0); // Lúc 19h20
            if (scheduledTime <= now)
            {
                scheduledTime = scheduledTime.AddDays(1); // Nếu đã qua thời gian đã lên lịch, chuyển sang ngày tiếp theo
            }
            TimeSpan delay = scheduledTime - now;
            return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
        }

        private void DoWork(object state)
        {
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _localTimeZone);
            _inventoryConfigService.CreateSystemNotification();
            _logger.LogInformation("Worker running at: {time}", localTime);
            _timer.Change(GetDelay(), TimeSpan.FromHours(24)); // Đặt lại timer cho ngày tiếp theo
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
