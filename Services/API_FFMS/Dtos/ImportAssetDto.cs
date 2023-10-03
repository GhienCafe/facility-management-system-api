﻿using MainData.Entities;

namespace API_FFMS.Dtos
{
    public class ImportAssetDto
    {
        public string AssetName { get; set; } = string.Empty;
        public string? AssetCode { get; set; }
        public string? TypeCode { get; set; }
        public string? Model { get; set; }
        public AssetStatus Status { get; set; }
        public int? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public string? IsRented { get; set; }
        public string? IsMovable { get; set; }
    }

    public class ImportAssetToRoomDto
    {
        public string? RoomName { get; set; }
        public string? AssetCode { get; set; }
        public string? Description { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime? FromDate { get; set; }
        public double Quantity { get; set; }
    }
}
