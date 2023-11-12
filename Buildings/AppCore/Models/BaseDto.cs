using System.Text.Json.Serialization;
using AppCore.Configs;

namespace AppCore.Models;

public class BaseQueryDto
{
    public Guid Id { get; set; }
    public string? Keyword { get; set; }
    public DateTime? CreateAtFrom { get; set; }
    public DateTime? CreateAtTo { get; set; }

    private const int MaxPageCount = 1000; // Max page size

    private const int DefaultPageSize = 25; // Default page size

    private int _pageSize = DefaultPageSize;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? DefaultPageSize : (value > MaxPageCount ? MaxPageCount : value);
    }

    public string OrderBy { get; set; } = "CreatedAt desc";

    public int Skip()
    {
        return PageSize * (Page - 1);
    }
}

public class BaseDto
{
    public Guid Id { get; set; }
    [JsonIgnore] public Guid CreatorId { get; set; }
    [JsonIgnore] public Guid EditorId { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime CreatedAt { get; set; }
    [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
    public DateTime EditedAt { get; set; }
    public AccountCreator? Creator { get; set; }
    public AccountCreator? Editor { get; set; }
}

public class AccountCreator
{
    public Guid Id { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
}