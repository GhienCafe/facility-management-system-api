using AppCore.Models;
using MainData.Entities;
using Microsoft.AspNetCore.Identity;

namespace API_FFMS.Dtos;

public class MaintenanceDto : BaseRequestDto {
    public AssetBaseDto? Asset { get; set; }
    public UserBaseDto? User { get; set; }
    public AssetTypeDto? AssetType { get; set; }
    public CategoryDto? Category { get; set; }
    public MediaFileDto? MediaFile { get; set; }
}

public class MediaFileDto
{
    public List<string>? Uri { get; set; }
    public FileType FileType { get; set; }
    public string? Content { get; set; }
    public Guid? ItemId { get; set; }
}

public class MaintenanceQueryDto : BaseRequestQueryDto
{
}

public class MaintenanceCreateDto : BaseRequestCreateDto
{
}

public class MaintenanceUpdateDto : BaseRequestUpdateDto
{
}