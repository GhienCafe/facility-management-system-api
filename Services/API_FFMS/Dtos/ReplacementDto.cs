namespace API_FFMS.Dtos;

public class ReplaceDto : BaseRequestDto {
    public Guid NewAssetId { get; set; }
    public AssetBaseDto? Asset { get; set; }
    public AssetBaseDto? NewAsset { get; set; }
    public UserBaseDto? AssignTo { get; set; }
    public AssetTypeDto? AssetType { get; set; }
    public CategoryDto? Category { get; set; }
    public MediaFileDto? MediaFile { get; set; }
    public AssetLocation? AssetLocation { get; set; }
    public AssetLocation? NewAssetLocation { get; set; }
}


public class AssetLocation
{
    public string? RoomName { get; set; }
    public string? RoomCode { get; set; }
}

public class ReplaceCreateDto : BaseRequestCreateDto
{
    public Guid NewAssetId { get; set; }
}

public class ReplacementQueryDto : BaseRequestQueryDto { }