namespace AppCore.Models;

public class NotificationModel
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? UserId { get; set; }

}

public class Registration
{
    public string? Token { get; set; }
}

public class PriorityDto
{
    public bool Priority { get; set; }
}

public class ListToken
{
    public List<string>? Tokens { get; set; }
}

public class Request
{
    public ListToken? ListToken { get; set; }
    public NotificationModel? Notification { get; set; }
}