using MainData;
using Microsoft.EntityFrameworkCore;

namespace Worker_Notify.Workers;

public class InventoryWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public InventoryWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
         

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); 
        }
    }
}