using System.Linq.Expressions;
using AppCore.Models;
using MainData;
using MainData.Repositories;
using Microsoft.AspNetCore.Http;
using Worker_Notify.Dtos;

namespace Worker_Notify.Services;

public interface INotificationService : IBaseService
{
    //Task UpdateNotification();
}

public class NotificationService :BaseService, INotificationService
{
    public NotificationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    
    public async Task UpdateNotification()
    {
        var notifications = MainUnitOfWork.NotificationRepository.GetQuery()
            .Where(x => !x.DeletedAt.HasValue && x.IsSendAll == false)  
            .ToList();
        
        foreach (var noti in notifications)
        {
            noti!.IsSendAll = true;
            if (await MainUnitOfWork.NotificationRepository.UpdateAsync(noti, noti.UserId, CurrentDate))
            {
                Console.WriteLine("update noti sucessfull");
            }
        }
    }

    public async Task<ApiResponses<NotificationDto>> GetCampus(NotificationQueryDto queryDto)
    {
        var notification = await MainUnitOfWork.NotificationRepository.FindResultAsync<NotificationDto>(
            new Expression<Func<MainData.Entities.Notification, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.IsSendAll == false,
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        notification.Items = await _mapperRepository.MapCreator(notification.Items.ToList());
        
        return ApiResponses<NotificationDto>.Success(
            notification.Items,
            notification.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(notification.TotalCount / (double)queryDto.PageSize)
        );
    }


}