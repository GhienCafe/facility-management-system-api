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

}
