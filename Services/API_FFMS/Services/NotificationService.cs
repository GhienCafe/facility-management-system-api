using System.Linq.Expressions;
using API_FFMS.Dtos;
using MainData;
using MainData.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

public interface INotificationService : IBaseService
{
    Task<ApiResponse> ReadNotification(Guid id);
    Task<ApiResponse> ReadAllNotification();
    Task<ApiResponses<NotifcationBaseDto>> GetNotificationOfAPerson(NotificationQueryDto queryDto);
}
public class NotificationService : BaseService, INotificationService
{
    public NotificationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> ReadNotification(Guid id)
    {
        var notification = await MainUnitOfWork.NotificationRepository.GetQuery().SingleOrDefaultAsync(notification => !notification!.DeletedAt.HasValue && AccountId == notification.UserId && notification.Id == id);
        notification!.IsRead = true;
        if (! await MainUnitOfWork.NotificationRepository.UpdateAsync(notification, AccountId, CurrentDate))
        {
            throw new ApiException("Thông tin đã sai");
        }
        return ApiResponse.Success("Đã đọc");
    }
    public async Task<ApiResponse> ReadAllNotification()
    {
        var notification = await MainUnitOfWork.NotificationRepository.GetQuery().Where(notification => !notification!.DeletedAt.HasValue && AccountId == notification.UserId).ToListAsync();
        notification.ForEach(notification => notification.IsRead = true);
        if (! await MainUnitOfWork.NotificationRepository.UpdateAsync(notification, AccountId, CurrentDate))
        {
           throw new ApiException("Thông tin đã sai");
        }
        return ApiResponse.Success("Đã đọc toàn bộ");
    }
    
    public async Task<ApiResponses<NotifcationBaseDto>> GetNotificationOfAPerson(NotificationQueryDto queryDto)
    {
        var notificationQueryable = MainUnitOfWork.NotificationRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.UserId == AccountId);

        if (queryDto.IsRead != null)
        {
            notificationQueryable = notificationQueryable.Where(x => x!.IsRead == queryDto.IsRead);
        }
        
        if (queryDto.Type != null)
        {
            notificationQueryable = notificationQueryable.Where(x => x!.Type == queryDto.Type);
        }
        
        if (queryDto.Status != null)
        {
            notificationQueryable = notificationQueryable.Where(x => x!.Status == queryDto.Status);
        }

        notificationQueryable = notificationQueryable.OrderByDescending(x => x!.CreatedAt);

        var totalCount = await notificationQueryable.CountAsync();

        notificationQueryable = notificationQueryable.Skip(queryDto.Skip())
            .Take(queryDto.PageSize);

        var notifications = await notificationQueryable.Select(x => new NotifcationBaseDto
        {
            Id = x!.Id,
            Content = x.Content,
            IsRead = x.IsRead,
            Type = x.Type,
            TypeObj = x.Type.GetValue(),
            Status = x.Status,
            StatusObj = x.Status.GetValue(),
            UserId = x.UserId,
            Title = x.Title,
            ShortContent = x.ShortContent,
            ItemId = x.ItemId,
            EditedAt = x.CreatedAt,
            CreatedAt = x.CreatedAt,
            EditorId = x.EditorId ?? Guid.Empty,
            CreatorId = x.CreatorId ?? Guid.Empty
        }).ToListAsync();
        
        notifications = await _mapperRepository.MapCreator(notifications);

        return ApiResponses<NotifcationBaseDto>.Success(
            notifications,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling((double)totalCount / queryDto.PageSize)
        );
    }
    
    private static Task InitializeFirebase()
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            string projectId = EnvironmentExtension.GetProjectIdFirebase();
            string privateKeyId = EnvironmentExtension.GetPrivateKeyIdFirebase();
            string privateKey = EnvironmentExtension.GetPrivateKeyFirebase();
            string clientEmail = EnvironmentExtension.GetClientEmailFireBase();

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson($@"
        {{
            ""type"": ""service_account"",
            ""project_id"": ""{projectId}"",
            ""private_key_id"": ""{privateKeyId}"", // Sử dụng giá trị từ biến môi trường
            ""private_key"": ""{privateKey}"",
            ""client_email"": ""{clientEmail}""
        }}")
            });
        }

        return Task.CompletedTask;
    }

}


