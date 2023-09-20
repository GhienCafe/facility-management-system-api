using AppCore.Models;

namespace API_FFMS.Dtos
{
    public class NotificationDto : BaseDto
    {
        public string? Title { get; set; } = null!;
        public string? Body { get; set; }= null!;
    }

    public class RegistrationDto
    {
        public string? Token { get; set; }= null!;
    }

    public class PriorityDto
    {
        public bool Priority { get; set; }
    }

    public class ListToken
    {
        public List<string> Tokens { get; set; }= null!;
    }

    public class RequestDto
    {
        public ListToken? ListToken { get; set; }= null!;
        public NotificationDto? Notification { get; set; }= null!;
    }

    public class NotificationQueryDto : BaseQueryDto
    {
        
    }
}