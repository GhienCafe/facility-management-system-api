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
    Task<ApiResponses<NotifcationDetail>> GetNotification(NotificationQueryDto queryDto);
}
public class NotificationService : BaseService, INotificationService
{
    public NotificationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<NotifcationDetail>> GetNotification(NotificationQueryDto queryDto)
    {
        var notifications = await MainUnitOfWork.NotificationRepository.FindResultAsync<MainData.Entities.Notification>(
            new Expression<Func<MainData.Entities.Notification, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.UserId == AccountId
            }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        var notificationDtos = new List<NotifcationDetail>();

        foreach (var notification in notifications.Items)
        {
            var notifcationDetail = new NotifcationDetail
            {
                Title = notification.Title,
                Content = notification.Content,
                IsRead = notification.IsRead,
                Type = notification.Type.GetValue(),
                ShortContent = notification.ShortContent,
                CreatorId = notification.CreatorId ?? Guid.Empty,
                Id = notification.Id,
                ItemId = notification.ItemId,
            };
            

            if (notification.Type == NotificationType.Maintenance)
            {
                var maintenance = await MainUnitOfWork.MaintenanceRepository
                    .GetQuery().SingleOrDefaultAsync(x => x!.Id == notification.ItemId);

                if (maintenance != null)
                {
                    notifcationDetail.Maintenance = new MaintenanceDto
                    {
                        Id = maintenance.Id,
                        RequestedDate = maintenance.RequestedDate,
                        CompletionDate = maintenance.CompletionDate,
                        Description = maintenance.Description,
                        Note = maintenance.Note,
                        AssignedTo = maintenance.AssignedTo,
                        AssetId = maintenance.AssetId,
                        CreatorId = maintenance.CreatorId??Guid.Empty,
                        CreatedAt = maintenance.CreatedAt,
                        EditedAt = maintenance.EditedAt,
                        EditorId = maintenance.EditorId??Guid.Empty
                        // Thêm các thuộc tính khác của MaintenanceDto
                    };

                    notifcationDetail.Maintenance = await _mapperRepository.MapCreator(notifcationDetail.Maintenance);

                }
            }
            else if (notification.Type == NotificationType.Replacement)
            {
                var replacement = await MainUnitOfWork.ReplacementRepository.GetQuery()
                    .SingleOrDefaultAsync(x => x!.Id == notification.ItemId);

                if (replacement != null)
                {
                    notifcationDetail.Replacement = new ReplacementDto
                    {
                        Id = replacement.Id,
                        RequestedDate = replacement.RequestedDate,
                        CompletionDate = replacement.CompletionDate,
                        Description = replacement.Description,
                        Note = replacement.Note,
                        Reason = replacement.Reason,
                        Status = replacement.Status, // Phải chuyển đổi ReplacementStatus sang EnumValue
                        AssignedTo = replacement.AssignedTo,
                        AssetId = replacement.AssetId,
                        CreatorId = replacement.CreatorId??Guid.Empty,
                        CreatedAt = replacement.CreatedAt,
                        NewAssetId = replacement.NewAssetId,
                        EditedAt = replacement.EditedAt,
                        EditorId = replacement.EditorId??Guid.Empty
                        // Thêm các thuộc tính khác của ReplacementDto
                    };
                    notifcationDetail.Replacement = await _mapperRepository.MapCreator(notifcationDetail.Replacement);
                }
            }

            notificationDtos.Add(notifcationDetail);
        }

        return ApiResponses<NotifcationDetail>.Success(
            notificationDtos,
            notifications.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling((double)notifications.TotalCount / queryDto.PageSize)
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


