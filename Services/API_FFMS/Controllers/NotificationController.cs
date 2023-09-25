using Microsoft.AspNetCore.Mvc;
using MainData.Middlewares;
using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation("Get list notification")]
        public async Task<ApiResponses<NotifcationDetail>> GetListNotification([FromQuery] NotificationQueryDto queryDto)
        {
            return await _notificationService.GetNotification(queryDto);
        }

        [HttpPost("{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage(string token, [FromBody] NotificationDto fcmNotiMessage)
        {
            await _notificationService.SendSingleMessage(fcmNotiMessage, token);
            return Ok("Message sent successfully");
        }

        [HttpPost("multicast")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMulticastMessage([FromBody] RequestDto request)
        {
            await _notificationService.SendMultipleMessages(request);
            return Ok("Multicast message sent successfully");
        }
    }
}
