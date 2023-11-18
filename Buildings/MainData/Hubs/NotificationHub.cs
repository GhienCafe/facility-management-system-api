// using System.Linq.Expressions;
// using AppCore.Extensions;
// using MainData.Entities;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.SignalR;

// namespace MainData.Hubs;

// public class NotificationHub : Hub
// {
//     private readonly MainUnitOfWork _mainUnitOfWork;
//     internal readonly IHttpContextAccessor HttpContextAccessor;
//     internal Guid? AccountId;

//     public NotificationHub(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor)
//     {
//         _mainUnitOfWork = mainUnitOfWork;
//         HttpContextAccessor = httpContextAccessor;
//         AccountId = httpContextAccessor.HttpContext?.User.GetUserId();
//     }

//     public async Task SendNotificationsToClients()
//     {
//         var notifications = await _mainUnitOfWork.NotificationRepository.FindAsync(new Expression<Func<Notification, bool>>[]
//         {
//             x => x.UserId == AccountId,
//             x => !x.DeletedAt.HasValue
//         }, "CreatedAt desc");
//         // Send notifications to connected clients
//         await Clients.All.SendAsync("ReceiveNotification", notifications);
//     }
// }   