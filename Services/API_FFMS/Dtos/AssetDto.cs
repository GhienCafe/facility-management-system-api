using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos
{
    public class AssetDto
    {
        public Guid TypeId { get; set; }
        public string AssetName { get; set; } = null!;
        public string? AssetCode { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime ManufacturingYear { get; set; }
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
        public AssetStatus Status { get; set; }
        public DateTime ManufacturingYear { get; set; }
        public string? SerialNumber { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
    }

    public class AssetCreateDto
    {
        [Required(ErrorMessage = "Type ID is required.")]
        public Guid TypeId { get; set; }

        [Required(ErrorMessage = "Asset name is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Asset name must be between 1 and 255 characters.")]
        public string AssetName { get; set; } = null!;

        [Required(ErrorMessage = "Asset code is required.")]
        [StringLength(255, ErrorMessage = "Asset code cannot exceed 255 characters.")]
        public string? AssetCode { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public AssetStatus Status { get; set; }

        [Required(ErrorMessage = "Manufacturing Year is required.")]
        public DateTime ManufacturingYear { get; set; }

        [Required(ErrorMessage = "Serial number is required.")]
        public string? SerialNumber { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public double Quantity { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
    }

    public class AssetUpdateDto
    {
        public Guid? TypeId { get; set; }

        [StringLength(255, MinimumLength = 1, ErrorMessage = "Asset Name must be between 1 and 255 characters.")]
        public string AssetName { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Asset Code cannot exceed 255 characters.")]

        //public AssetStatus? Status { get; set; }

        public DateTime? ManufacturingYear { get; set; }

        public string? SerialNumber { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public double? Quantity { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
    }

    public class AssetQueryDto : BaseQueryDto
    {
        public string? AssetName { get; set; }
        public string? AssetCode { get; set; }
    }
}
