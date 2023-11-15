using AppCore.Data.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MainData.Middlewares;

namespace API_FFMS.Controllers
{
    public class NotificationHubController : BaseController
    {
        private readonly IHubContext<NotificationHub> _chatHubContext;
        private readonly ILogger<NotificationHubController> _logger;
        private readonly NotificationHub _notificationHub; // Adding the instance of NotificationHub

        public NotificationHubController(IHubContext<NotificationHub> chatHubContext, ILogger<NotificationHubController> logger, NotificationHub notificationHub)
        {
            _chatHubContext = chatHubContext;
            _logger = logger;
            _notificationHub = notificationHub; // Injecting NotificationHub instance
        }
        
        [HttpPost("test")]
        [AllowAnonymous]
        public async Task<IActionResult> BroadcastMessage(string user, string message)
        {
            try
            {
                // Send the received message to SignalR clients using the SendNotification method in NotificationHub
                await _notificationHub.SendNotification(message);

                return Ok("Message broadcasted to SignalR clients");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting message to SignalR clients");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}