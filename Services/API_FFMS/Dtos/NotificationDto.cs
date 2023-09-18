namespace API_FFMS.Dtos
{
    public class NotificationDto
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
    }

    public class RegistrationDto
    {
        public string? Token { get; set; }
    }

    public class PriorityDto
    {
        public bool Priority { get; set; }
    }

    public class ListToken
    {
        public List<string> Tokens { get; set; }
    }

    public class RequestDto
    {
        public ListToken? ListToken { get; set; }
        public NotificationDto? Notification { get; set; }
    }
}