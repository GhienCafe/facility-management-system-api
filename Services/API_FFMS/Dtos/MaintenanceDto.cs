using MainData.Entities;

namespace API_FFMS.Dtos;

public class MaintenanceDto : BaseRequestDto {
    public AssetBaseDto? Asset { get; set; }
    public UserBaseDto? User { get; set; }
    public AssetTypeDto? AssetType { get; set; }
    public CategoryDto? Category { get; set; }
    public MediaFileDto? MediaFile { get; set; }
}

public class MaintenanceQueryDto : BaseRequestQueryDto
{
}

public class MaintenanceCreateDto : BaseRequestCreateDto { }

public class MaintenanceUpdateDto : BaseRequestUpdateDto
{
}