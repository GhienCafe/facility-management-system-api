using AppCore.Configs;
using AppCore.Models;
using MainData.Entities;
using Newtonsoft.Json;

namespace API_FFMS.Dtos
{

    public class RoomAssetBaseDto : BaseDto
    {
        public Guid RoomId { get; set; }
        public Guid AssetId { get; set; }
        public AssetStatus Status { get; set; }
        [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
        public DateTime? FromDate { get; set; }
        [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
        public DateTime? ToDate { get; set; }
        public double? Quantity { get; set; }
        public RoomBaseDto? Room { get; set; }
        public AssetBaseDto? Asset { get; set; }
    }
    
    public class RoomAssetCreateBaseDto 
    {
        public Guid RoomId { get; set; }
        public Guid? AssetId { get; set; }
        public double? Quantity { get; set; }
    }
    
    public class RoomAssetUpdateBaseDto
    {
        public Guid? RoomId { get; set; }
        public Guid? AssetId { get; set; }
        public AssetStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double? Quantity { get; set; }
    }
    
    public class RoomAssetDetailDto : BaseDto
    {
        public Guid? RoomId { get; set; }
        public Guid? AssetId { get; set; }
        public AssetStatus Status { get; set; }
        [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
        public DateTime? FromDate { get; set; }
        [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
        public DateTime? ToDate { get; set; }
        public double? Quantity { get; set; }
        public RoomBaseDto? Room { get; set; }
        public AssetBaseDto? Asset { get; set; }
    }
    
    public class AssetTrackingDto : BaseDto
    {
        public AssetStatus? Status { get; set; }
        [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
        public DateTime? FromDate { get; set; }
        [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
        public DateTime? ToDate { get; set; }
        public RoomBaseDto? Room { get; set; }

    }

    public class RoomTrackingDto
    {
        public Guid? RoomId { get; set; }
        public AssetStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public AssetDto? Asset { get; set; }
    }

    public class RoomTrackingQueryDto : BaseQueryDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class RoomAssetCreateDto
    {
        public Guid? RoomId { get; set; }
        public Guid? AssetId { get; set; }
        public AssetStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
    
    
    public class RoomAssetDto : BaseDto
    {
        public AssetStatus? Status { get; set; }
        public EnumValue? StatusObj { get; set; }
        public double? Quantity { get; set; }
        [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
        public DateTime? FromDate { get; set; }
        [JsonConverter(typeof(LocalTimeZoneDateTimeConverter))]
        public DateTime? ToDate { get; set; }
        public AssetDto? Asset { get; set; }
    }

    public class RoomAssetQueryDto : BaseQueryDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsInCurrent { get; set; }
        public AssetStatus? Status { get; set; }
    }

}
