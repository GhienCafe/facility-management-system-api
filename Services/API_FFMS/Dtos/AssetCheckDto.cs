using AppCore.Models;

namespace API_FFMS.Dtos;

public class AssetCheckDto : BaseRequestDto
{
    public bool IsVerified { get; set; }
    public AssetBaseDto? Asset { get; set; }
    public RoomBaseDto? Location { get; set; }
    public MediaFileDto? MediaFile { get; set; }
}

public class AssetCheckQueryDto : BaseRequestQueryDto { }

public class AssetCheckCreateDto : BaseRequestCreateDto { }

public class AssetCheckUpdateDto : BaseRequestUpdateDto { }