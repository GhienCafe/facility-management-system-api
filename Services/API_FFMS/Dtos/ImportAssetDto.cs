﻿using MainData.Entities;

namespace API_FFMS.Dtos
{
    public class ImportAssetDto
    {
        public string AssetName { get; set; } = string.Empty;
        public string? AssetCode { get; set; }
        public string? CategoryCode { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
    }
}