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
    Task SendSingleMessage(NotificationDto noti, string token);
    Task SendMultipleMessages(RequestDto request);
    Task<ApiResponse> ReadNotification(Guid id);
    Task<ApiResponses<NotifcationBaseDto>> GetNotificationOfAPerson(NotificationQueryDto queryDto);
}
public class NotificationService : BaseService, INotificationService
{
    public NotificationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> ReadNotification(Guid id)
    {
        var notification = await MainUnitOfWork.NotificationRepository.GetQuery().SingleOrDefaultAsync(notification => !notification!.DeletedAt.HasValue && notification.Id == id);
        notification!.IsRead = true;
        if (! await MainUnitOfWork.NotificationRepository.UpdateAsync(notification, AccountId, CurrentDate))
        {
            return ApiResponse.Failed("Thông tin đã sai");
        }
        return ApiResponse.Success("Đã đọc");
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

    public async Task SendSingleMessage(NotificationDto noti, string token)
    {
        // Khởi tạo Firebase nếu chưa được khởi tạo
        await InitializeFirebase();

        var message = new Message()
        {
            Data = new Dictionary<string, string>()
            {
                { "score", "850" },
                { "time", "1:00" },
            },
            Notification = new Notification
            {
                Title = noti.Title,
                Body = noti.Body,
            },
            Token = token,
            Webpush = new WebpushConfig
            {
                Notification = new WebpushNotification
                {
                    Title = noti.Title,
                    Body = noti.Body,
                },
            },
        };

        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message).ConfigureAwait(false);

        var notification = new MainData.Entities.Notification()
        {
            Title = noti.Title,
            Content = noti.Body,
            IsRead = false,
            UserId = AccountId
        };
        if (string.IsNullOrEmpty(response))
        {
            throw new ApiException("Server error for not valid sent message", StatusCode.BAD_REQUEST);
        }
        if (!await MainUnitOfWork.NotificationRepository.InsertAsync(notification, AccountId, CurrentDate))
        {
            throw new ApiException("Server error for not insert notification", StatusCode.BAD_REQUEST);
        }
    }

    public async Task SendMultipleMessages(RequestDto request)
    {
        // Khởi tạo Firebase nếu chưa được khởi tạo
        await InitializeFirebase();

        var message = new MulticastMessage()
        {
            Tokens = request.ListToken!.Tokens,
            Notification = new Notification
            {
                Title = request.Notification?.Title,
                Body = request.Notification?.Body,
            },
            Data = new Dictionary<string, string>()
            {
                { "content_type", "notification" },
                { "value", "2" }
            },
            Webpush = new WebpushConfig
            {
                Notification = new WebpushNotification
                {
                    Title = request.Notification?.Title,
                    Body = request.Notification?.Body,
                },
            },
        };

        BatchResponse response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(false);

        if (response.FailureCount > 0)
        {
            Console.WriteLine($"Failed to send {response.FailureCount} messages");

            for (int i = 0; i < response.Responses.Count; i++)
            {
                if (!response.Responses[i].IsSuccess)
                {
                    string errorToken = request.ListToken.Tokens[i];
                    Console.WriteLine($"Failed to send message to token: {errorToken}");
                }
            }

            throw new Exception("Server error for not valid sent message");
        }
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


