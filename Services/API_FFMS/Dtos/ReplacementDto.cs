using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class ReplaceDto : BaseRequestDto
{
    public Guid? NewAssetId { get; set; }
    public AssetStatus? StatusBefore { get; set; }
    public EnumValue? StatusBeforeObj { get; set; }
    public AssetBaseDto? Asset { get; set; }
    public AssetBaseDto? NewAsset { get; set; }
    public UserBaseDto? AssignTo { get; set; }
    public AssetTypeDto? AssetType { get; set; }
    public CategoryDto? Category { get; set; }
    public List<MediaFileDetailDto>? RelatedFiles { get; set; }
    public AssetLocation? AssetLocation { get; set; }
    public AssetLocation? NewAssetLocation { get; set; }
}

public class AssetReplaceDto
{
    public string AssetName { get; set; } = null!;
    public string? AssetCode { get; set; }
    public bool? IsMovable { get; set; }
    public AssetStatus? Status { get; set; }
    public EnumValue? StatusObj { get; set; }
    public int? ManufacturingYear { get; set; }
    public string? SerialNumber { get; set; }
    public double? Quantity { get; set; }
    public string? Description { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ModelId { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsRented { get; set; }
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