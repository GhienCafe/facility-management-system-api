using API_FFMS.Dtos;
using API_FFMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_FFMS.Controllers
{
    public class PushNotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public PushNotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("{token}")]
        public async Task<IActionResult> SendMessage(string token, [FromBody] NotificationDto fcmNotiMessage)
        {
            try
            {
                await _notificationService.SendSingleMessage(fcmNotiMessage, token);
                return Ok("Message sent successfully");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ở đây, ví dụ ghi nhật ký
                Console.WriteLine($"Error sending message: {ex.Message}");
                return StatusCode(500, "Server error for not valid sent message");
            }
        }
        [HttpPost("multicast")]
        public async Task<IActionResult> SendMulticastMessage([FromBody] RequestDto request)
        {
            try
            {
                await _notificationService.SendMultipleMessages(request);
                return Ok("Multicast message sent successfully");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ở đây, ví dụ ghi nhật ký
                Console.WriteLine($"Error sending multicast message: {ex.Message}");
                return StatusCode(500, "Server error for not valid sent multicast message");
            }
        }
    }
}