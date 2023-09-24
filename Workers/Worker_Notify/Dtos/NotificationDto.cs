using AppCore.Models;
using MainData.Entities;

namespace Worker_Notify.Dtos;

public class NotificationDto : BaseDto
{
    public string? Title { get; set; } = null!;
    public string? Body { get; set; } = null!;
}

public class RegistrationDto
{
    public string? Token { get; set; } = null!;
}

public class PriorityDto
{
    public bool Priority { get; set; }
}

public class ListToken
{
    public List<string> Tokens { get; set; } = null!;
}

public class RequestDto
{
    public ListToken? ListToken { get; set; } = null!;
    public NotificationDto? Notification { get; set; } = null!;
}

public class NotificationUpdateDto
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public string? Title { get; set; }
    public string? ShortContent { get; set; }
    public string? Content { get; set; }
    public bool? IsRead { get; set; } 
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    
    public Guid? ItemId { get; set; }
}
public class NotificationQueryDto : BaseQueryDto
{

}
