using AppCore.Models;

namespace API_FFMS.Dtos;

public class AssetCheckDto : BaseRequestDto
{
    //public Guid RequestId { get; set; }
    //public Guid AssetId { get; set; }
    public bool IsVerified { get; set; }
    public AssetBaseDto? Asset { get; set; }
    public RoomBaseDto? Location { get; set; } 
}

//public class AssetCheckQueryDto : BaseQueryDto
//{
//    public Guid? RequestId { get; set; }
//    public Guid? AssetId { get; set; }
//    public bool? IsVerified { get; set; }
//}

public class AssetCheckQueryDto : BaseRequestQueryDto
{
    public string? RequestCode { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
}

public class AssetCheckCreateDto : BaseRequestCreateDto { }

public class AssetCheckUpdateDto : BaseRequestUpdateDto { }