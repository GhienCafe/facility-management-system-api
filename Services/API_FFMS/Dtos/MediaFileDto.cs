using MainData.Entities;

namespace API_FFMS.Dtos;

public class MediaFileDto
{
    public List<string>? Uri { get; set; }
    public FileType FileType { get; set; }
    public string? Content { get; set; }
    public Guid? ItemId { get; set; }
}

public class MediaFileCreateDto
{
    public string? FileName { get; set; }
    public string? Uri { get; set; }
}

public class MediaFileDetailDto
{
    public string? FileName { get; set; }
    public string? Uri { get; set; }
}