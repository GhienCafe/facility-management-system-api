namespace API_FFMS.Dtos;

public class RepairDto : BaseRequestDto
{
    public AssetBaseDto? Asset { get; set; }
    public UserBaseDto? User { get; set; }
    public AssetTypeDto? AssetType { get; set; }
    public CategoryDto? Category { get; set; }
    public List<MediaFileDetailDto>? RelatedFiles { get; set; }
    public List<MediaFileDto>? Reports { get; set; }
}

public class RepairCreateDto : BaseRequestCreateDto { }

public class RepairQueryDto : BaseRequestQueryDto { }

public class RepairUpdateDto : BaseRequestUpdateDto { }