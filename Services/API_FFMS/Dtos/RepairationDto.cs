using MainData.Entities;

namespace API_FFMS.Dtos;

public class RepairationDto : BaseRequestDto
{
    public AssetBaseDto? Asset { get; set; }
    public UserBaseDto? User { get; set; }
}

public class RepairationCreateDto : BaseRequestCreateDto { }

public class RepairationQueryDto : BaseRequestQueryDto
{
    public string? RequestCode { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
}