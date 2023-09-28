using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Math;

namespace API_FFMS.Dtos
{
    public class AssetDto : BaseDto
    {
        public string AssetName { get; set; } = null!;
        public string? AssetCode { get; set; }
        public bool IsMovable { get; set; }
        public EnumValue? Status { get; set; }
        public DateTime? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? ModelId { get; set; }
        public bool? IsRented { get; set; }
        public AssetTypeDto? Type { get; set; }
        public ModelDto? Model { get; set; }
    }

    public class AssetDetailDto : BaseDto
    {
        public string AssetName { get; set; } = null!;
        public string? AssetCode { get; set; }
        public bool IsMovable { get; set; }
        public EnumValue? Status { get; set; }
        public DateTime? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public bool? IsRented { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? ModelId { get; set; }
        //public bool? IsRented { get; set; }
        public AssetTypeDto? Type { get; set; }
        public ModelDto? Model { get; set; }
    }

    public class AssetCreateDto
    {
        [Required(ErrorMessage = "Tên trang bị không được trống")]
        public string AssetName { get; set; } = null!;
        public string? AssetCode { get; set; }
        public bool IsMovable { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double? Quantity { get; set; }
        public string? Description { get; set; }
        public bool? IsRented { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? ModelId { get; set; }
        //public bool? IsRented { get; set; }
        public AssetTypeDto? Type { get; set; }
        public ModelDto? Model { get; set; }
    }

    public class AssetUpdateDto
    {
        public string? AssetName { get; set; }
        public string? AssetCode { get; set; }
        public bool? IsMovable { get; set; }
        public AssetStatus? Status { get; set; }
        public DateTime? ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double? Quantity { get; set; }
        public string? Description { get; set; }
        public bool? IsRented { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? ModelId { get; set; }
    }

    public class AssetQueryDto : BaseQueryDto
    {
        public AssetStatus? Status { get; set; }
        public bool? IsMovable { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? ModelId { get; set; }
        public bool? IsRented { get; set; }
    }
}


