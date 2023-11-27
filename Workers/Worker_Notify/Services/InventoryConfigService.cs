using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Worker_Notify.Services;

public interface IInventoryConfigService : IBaseService
{
    Task CreateSystemNotification();
}

public class InventoryConfigService : BaseService, IInventoryConfigService
{
    public InventoryConfigService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task CreateSystemNotification()
    {
        var inventoryConfig = await MainUnitOfWork.InventoryCheckConfigRepository.FindOneAsync(null);
        
        if (inventoryConfig != null)
        {
            var currentDate = DateTime.UtcNow;
            var checkDates = await MainUnitOfWork.InventoryDetailConfigRepository
                .GetQuery()
                .Where(x => x.InventoryConfigId == inventoryConfig.Id && x.InventoryDate > currentDate)
                .Select(x => x.InventoryDate)
                .ToListAsync();
            
            var userIds = await MainUnitOfWork.UserRepository.GetQuery()
                .Where(x => x.Role == UserRole.Manager).Select(x => x.Id).ToListAsync();
                
            foreach (var notification in from date in checkDates where date.Date.Subtract(currentDate.Date).Days <= inventoryConfig.NotificationDays from userId in userIds select new Notification
                     {
                         Content = "Sắp tới đợt kiểm kê đã đặt",
                         Status = NotificationStatus.Waiting,
                         Type = NotificationType.Info,
                         IsRead = false,
                         Title = "Nhắc lịch kiểm kê",
                         ShortContent = "Nhắc lịch kiểm kê",
                         UserId = userId
                     })
            {
            }
        }
    }
}
