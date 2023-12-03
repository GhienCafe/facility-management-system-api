namespace API_FFMS.Dtos;

public class AssetCheckDto : BaseRequestDto
{
    public bool? IsVerified { get; set; }
    public AssetBaseDto? Asset { get; set; }
    public RoomBaseDto? Location { get; set; }
    public UserBaseDto? AssignTo { get; set; }
    public AssetTypeDto? AssetType { get; set; }
    public CategoryDto? Category { get; set; }
    public List<MediaFileDetailDto>? RelatedFiles { get; set; }
    public List<MediaFileDto>? Reports { get; set; }
}

public class AssetCheckQueryDto : BaseRequestQueryDto { }

public class AssetCheckCreateDto : BaseRequestCreateDto { }

public class AssetCheckUpdateDto : BaseRequestUpdateDto { }

public class AssetCheckUpdateStatusDto : BaseUpdateStatusDto
{
    public bool? IsVerified { get; set; }
}