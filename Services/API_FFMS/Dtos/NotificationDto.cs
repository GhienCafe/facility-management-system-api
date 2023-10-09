using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos
{
    public class NotificationDto
    {
        public string? Title { get; set; } = null!;
        public string? Body { get; set; }= null!;
    }

    public class NotifcationDetail : BaseDto
    {
        public Guid? UserId { get; set; }
        public string? Title { get; set; }
        public string? ShortContent { get; set; }
        public string? Content { get; set; }
        public bool? IsRead { get; set; } 
        public EnumValue? Type { get; set; }
        public Guid? ItemId { get; set; }
        public ReplacementDto? Replacement { get; set; }
        public MaintenanceDto? Maintenance { get; set; }
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