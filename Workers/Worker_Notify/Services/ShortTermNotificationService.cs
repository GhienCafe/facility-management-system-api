using System.Linq.Expressions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Http;

namespace Worker_Notify.Services
{
    using AppCore.Extensions;

    public interface IShortTermNotificationService : IBaseService
    {
        Task SendTask();
    }

    public class ShortTermNotificationService : BaseService, IShortTermNotificationService
    {
        private readonly ISendNotification _sendNotification;

        public ShortTermNotificationService(
            MainUnitOfWork mainUnitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperRepository mapperRepository,
            ISendNotification sendNotification)
            : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _sendNotification = sendNotification;
        }

        public async Task SendTask()
        {
            try
            {
                var notificationsToSend = await MainUnitOfWork.NotificationRepository.FindAsync(
                    filters: new Expression<Func<Notification, bool>>[]
                    {
                        x => x.DeletedAt.HasValue == false && x.Status == NotificationStatus.Waiting
                    },
                    null
                );
            
                var userIds = notificationsToSend.Select(notification => notification!.UserId).Distinct().ToList();
                var tokens = await MainUnitOfWork.TokenRepository.FindAsync(
                    filters: new Expression<Func<Token, bool>>[]
                    {
                        x => x.Type == TokenType.DeviceToken && userIds.Contains(x.UserId)
                    },
                    orderBy: null
                );
            
                var tokenDictionary = tokens
                    .Where(token => !string.IsNullOrEmpty(token!.AccessToken))
                    .ToDictionary(token => token!.UserId, token => token!.AccessToken);
            
                var sendNotifications = notificationsToSend
                    .Where(notification => notification!.UserId.HasValue && tokenDictionary.ContainsKey(notification.UserId.Value))
                    .Select(notification => new NotificationModel
                    {
                        Title = notification!.Title,
                        Body = notification.Content,
                        UserId = notification.UserId.ToString()
                    })
                    .ToList();
                
                var distinctTokenAccessTokens = sendNotifications
                    .Where(notification =>
                        notification.UserId != null &&
                        tokenDictionary.ContainsKey(Guid.Parse(notification.UserId))
                    )
                    .Select(notification => tokenDictionary[Guid.Parse(notification.UserId!)])
                    .Distinct()
                    .ToList();
            
                Console.WriteLine($"DistinctTokenAccessTokens count: {distinctTokenAccessTokens.Count}");
                var groupedNotifications = sendNotifications
                    .GroupBy(n => n.UserId)
                    .ToList();
                
                foreach (var group in groupedNotifications)
                {
                    var userId = group.Key;
                    var notifications = group.ToList();
            
                    if (!tokenDictionary.TryGetValue(Guid.Parse(userId!), out var tokenAccessToken) || string.IsNullOrEmpty(tokenAccessToken))
                    {
                        Console.WriteLine("Error send firebase message");
                        continue;
                    }
            
                    List<string> tokenAccessTokens = notifications
                        .Select(x => tokenAccessToken)
                        .ToList();
                    if (notifications.Count > 1)
                    {
                        Console.WriteLine($"Sending multicast notification to user:{userId}");
            
                        await _sendNotification.SendFirebaseMulticastMessage(new Request
                        {
                            ListToken = new ListToken { Tokens = tokenAccessTokens },
                            Notification = new NotificationModel
                            {
                                Title = notifications.FirstOrDefault()?.Title,
                                Body = notifications.FirstOrDefault()?.Body,
                            },
                        });
                        
                    }
                    else if (notifications.Count == 1)
                    {
                        Console.WriteLine($"Sending notification to user:{userId}");
            
                        await _sendNotification.SendFirebaseMessage(notifications.FirstOrDefault()!, new Registration { Token = tokenAccessToken });
                    }
                }
                
                notificationsToSend = notificationsToSend.Select(notification => {
                    notification.Status = NotificationStatus.Sent;
                    return notification;
                }).ToList();
            
            
                if (!await MainUnitOfWork.NotificationRepository.UpdateAsync(notificationsToSend, Guid.Empty, CurrentDate))
                    Console.WriteLine("Update fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendAndMarkAsSent: {ex.Message}");
            }
        }
    }
}
