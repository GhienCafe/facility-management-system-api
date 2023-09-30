using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos
{
    public class AssetTypeDto: BaseDto
    {
        public string? TypeCode { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public EnumValue? Unit { get; set; }
    }

    public class AssetTypeDetailDto : BaseDto
    {
        public string TypeCode { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public string? Description { get; set; }
        public EnumValue? Unit { get; set; }
    }

    public class AssetTypeCreateDto
    {
        [Required(ErrorMessage = "Asset type code is required.")]
        [StringLength(255, ErrorMessage = "Asset type code cannot exceed 255 characters.")]
        public string TypeCode { get; set; } = null!;

        [Required(ErrorMessage = "Asset Name is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Asset type name must be between 1 and 255 characters.")]
        public string TypeName { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        public Unit Unit { get; set; }
    }

    public class AssetTypeUpdateDto
    {
        public string? TypeName { get; set; } = null!;
        public string? Description { get; set; }
        public Unit? Unit { get; set; }
    }

    public class AssetTypeQueryDto : BaseQueryDto
    {
        public string? Keyword { get; set; }
    }
}
