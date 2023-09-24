using System.Linq.Expressions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Http;

namespace Worker_Notify.Services
{
    using AppCore.Extensions;

    public interface IPushNotificationService : IBaseService
    {
        Task SendAndMarkAsSent();
    }

    public class PushNotificationService : BaseService, IPushNotificationService
    {
        private readonly ISendNotification _sendNotification;

        public PushNotificationService(
            MainUnitOfWork mainUnitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperRepository mapperRepository,
            ISendNotification sendNotification)
            : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _sendNotification = sendNotification;
        }

        public async Task SendAndMarkAsSent()
        {
            try
            {
                var notificationsToSend = await MainUnitOfWork.NotificationRepository.FindAsync(
                    filters: new Expression<Func<Notification, bool>>[]
                    {
                        x => x.DeletedAt.HasValue == false && x.IsRead == false
                    },
                    orderBy: null
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
                        Body = notification.Content, // Use the 'Content' property since 'Body' may not be available
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

                Console.WriteLine($"TokenDictionary count: {tokenDictionary.Count}");
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

                // foreach (var notification in notificationsToSend)
                // {
                //     await UpdateNotificationAsync(notification!.Id);
                // }
                
                notificationsToSend.Select(notification => {
                    notification!.IsRead = true;
                    return notification;
                });

                if (!await MainUnitOfWork.NotificationRepository.UpdateAsync(notificationsToSend, Guid.Empty, CurrentDate))
                    Console.WriteLine("Update fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendAndMarkAsSent: {ex.Message}");
            }
        }
        private async Task UpdateNotificationAsync(Guid id)
        {
            try
            {
                // Fetch the notification by its unique identifier (e.g., notificationDto.UserId)
                var notification = await MainUnitOfWork.NotificationRepository.FindOneAsync(id);

                if (notification == null)
                    throw new ApiException("Notification not found", StatusCode.NOT_FOUND);

                // Set IsRead to true unconditionally
                notification.IsRead = true;

                // Perform the update in the database
                if (!await MainUnitOfWork.NotificationRepository.UpdateAsync(notification, notification.CreatorId, CurrentDate))
                    Console.WriteLine("Update fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating notification: {ex.Message}");
            }
        }


        public async Task UpdateNotificationAsync(Notification notification,
            DateTime? currentDate)
        {
            try
            {
                if (notification.UserId != null)
                {
                    notification.IsRead = true;
                    await MainUnitOfWork.NotificationRepository.UpdateAsync(notification, notification.UserId,
                        currentDate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating notification: {ex.Message}");
            }
        }
    }
}
