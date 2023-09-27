using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Math;

namespace API_FFMS.Dtos
{
    public class AssetDto : BaseDto
    {
        public Guid TypeId { get; set; }
        public string AssetName { get; set; } = null!;
        public string? AssetCode { get; set; }
        public bool IsMovable { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
    }

    public class AssetDetailDto : BaseDto
    {
        public Guid TypeId { get; set; }
        public string AssetName { get; set; } = null!;
        public string? AssetCode { get; set; }
        public bool IsMovable { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public bool? IsRented { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
    }

    public class AssetCreateDto
    {
        [Required]public Guid TypeId { get; set; }
        [Required]public string AssetName { get; set; } = null!;
        public string? AssetCode { get; set; }
        [Required]public bool IsMovable { get; set; }
        public DateTime? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        [Required]public double Quantity { get; set; }
        public string? Description { get; set; }
        public bool? IsRented { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
    }

    public class AssetUpdateDto
    {
        public Guid? TypeId { get; set; }
        public string? AssetName { get; set; } = null!;
        public string? AssetCode { get; set; }
        public bool? IsMovable { get; set; }
        public AssetStatus? Status { get; set; }
        public DateTime? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double? Quantity { get; set; }
        public string? Description { get; set; }
        public bool? IsRented { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
    }

    public class AssetQueryDto : BaseQueryDto
    {
        public string? AssetName { get; set; }
        public string? AssetCode { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public AssetStatus? Status { get; set; }
        public bool? IsMovable { get; set; }
        public bool? IsRented { get; set; }
    }
}


