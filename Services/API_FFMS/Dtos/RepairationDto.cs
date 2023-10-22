namespace API_FFMS.Dtos;

public class RepairationDto : BaseRequestDto
{
    public AssetBaseDto? Asset { get; set; }
    public UserBaseDto? User { get; set; }
    public AssetTypeDto? AssetType { get; set; }
    public CategoryDto? Category { get; set; }
    public MediaFileDto? MediaFile { get; set; }
}

public class RepairationCreateDto : BaseRequestCreateDto { }

public class RepairationQueryDto : BaseRequestQueryDto { }