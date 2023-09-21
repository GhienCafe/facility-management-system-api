using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos
{
    public class AssetTrackingDto
    {
        public Guid AssetId { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public RoomDto? Room { get; set; }
    }

    public class RoomTrackingDto
    {
        public Guid RoomId { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public AssetDto? Asset { get; set; }
    }

    public class RoomTrackingQueryDto : BaseQueryDto
    {
        public Guid AssetId { get; set; }
        public Guid RoomId { get; set; }
    }

    public class RoomAssetCreateDto
    {
        public Guid RoomId { get; set; }
        public Guid AssetId { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
    
    
    public class RoomAssetDto : BaseDto
    {
        public AssetStatus Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public AssetDto? AssetDto { get; set; }
    }

    public class RoomAssetQueryDto : BaseQueryDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsInCurrent { get; set; }
        public AssetStatus? Status { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; }
    }

}
