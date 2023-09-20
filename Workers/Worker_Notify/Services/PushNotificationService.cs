// using AppCore.Extensions;
// using MainData;
// using MainData.Repositories;
// using Microsoft.AspNetCore.Http;
//
// namespace Worker_Notify.Services
// {
//     public interface IPushNotificationService : IBaseService
//     {
//         Task SendAndMarkAsSent();
//     }
//
//     public class PushNotificationService :BaseService, IPushNotificationService
//     {
//         private readonly ISendNotification _sendNotification;
//         
//         public PushNotificationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository, ISendNotification sendNotification) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
//         {
//             _sendNotification = sendNotification;
//         }
//         
//         public async Task SendAndMarkAsSent()
//         {
//             try
//             {
//                 var notificationsToSend = MainUnitOfWork.NotificationRepository.GetQuery()
//                     .Where(x => !x.DeletedAt.HasValue && !x.IsSendAll)
//                     .ToList();
//
//                 foreach (var notification in notificationsToSend)
//                 {
//                     var token = MainUnitOfWork.TokenRepository.GetQuery()
//                         .SingleOrDefault(x => x != null && x!.UserId == notification.UserId);
//                     if (string.IsNullOrEmpty(token.AccessToken))
//                     {
//                         Console.WriteLine("device not found");
//                     }
//
//                     var registrationToken = new Registration { Token = token!.AccessToken };
//
//                     var sendNotification = new AppCore.Extensions.Notification
//                     {
//                         Title = notification.Title,
//                         Body = notification.Content,
//                     };
//                     await _sendNotification.SendFirebaseMessage(sendNotification, registrationToken);
//
//                     // Update the notification's IsSendAll flag to true
//                     notification.IsSendAll = true;
//                     if (!await MainUnitOfWork.NotificationRepository.UpdateAsync(notification, notification.UserId, CurrentDate))
//                     {
//                         Console.WriteLine("Error update");
//                     }
//
//                 }
//             }
//             catch (Exception ex)
//             {
//                 // Handle exceptions here and log or handle accordingly
//                 Console.WriteLine($"Error in SendAndMarkAsSent: {ex.Message}");
//             }
//         }
//
//     }
// }


using MainData.Entities;

namespace Worker_Notify.Services;
    
using AppCore.Extensions;
using MainData;
using MainData.Repositories;
using Microsoft.AspNetCore.Http;

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
            var notificationsToSend = MainUnitOfWork.NotificationRepository.GetQuery()
                .Where(x => !x.DeletedAt.HasValue && !x.IsSendAll)
                .ToList();

            foreach (var notification in notificationsToSend)
            {
                var token = MainUnitOfWork.TokenRepository.GetQuery()
                    .FirstOrDefault(x => x != null && x.UserId == notification.UserId && x.Type == TokenType.DeviceToken);

                if (token == null || string.IsNullOrEmpty(token.AccessToken))
                {
                    Console.WriteLine("Device not found for user: " + notification.UserId);
                    continue; // Skip this notification and continue with the next one
                }

                var registrationToken = new Registration { Token = token.AccessToken };

                var sendNotification = new AppCore.Extensions.Notification
                {
                    Title = notification.Title,
                    Body = notification.Content,
                };

                // Use the SendFirebaseMessage method from your ISendNotification implementation
                await _sendNotification.SendFirebaseMessage(sendNotification, registrationToken);

                // Update the notification's IsSendAll flag to true
                notification.IsSendAll = true;

                if (!await MainUnitOfWork.NotificationRepository.UpdateAsync(notification, notification.UserId, CurrentDate))
                {
                    Console.WriteLine("Error updating notification for user: " + notification.UserId);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here and log or handle accordingly
            Console.WriteLine($"Error in SendAndMarkAsSent: {ex.Message}");
        }
    }
}
