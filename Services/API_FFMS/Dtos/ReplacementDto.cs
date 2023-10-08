namespace API_FFMS.Dtos;

public class ReplaceDto : BaseRequestDto {
    public Guid NewAssetId { get; set; }
    public AssetBaseDto? Asset { get; set; }
    public AssetBaseDto? NewAsset { get; set; }
    public UserDto? User { get; set; }
}

public class ReplaceCreateDto : BaseRequestCreateDto
{
    public Guid NewAssetId { get; set; }
}

public class ReplacementQueryDto : BaseRequestQueryDto
{
    public string? RequestCode { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
}