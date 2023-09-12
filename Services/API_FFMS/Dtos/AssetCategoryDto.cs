using AppCore.Models;
using MainData.Entities;
using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos
{
    public class AssetCategoryDto
    {
        public string? CategoryCode { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public Unit Unit { get; set; }
    }

    public class AssetCategoryDetailDto : BaseDto
    {
        public string CategoryCode { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public Unit Unit { get; set; }
    }

    public class AssetCategoryCreateDto
    {
        [Required(ErrorMessage = "Asset category code is required.")]
        [StringLength(255, ErrorMessage = "Asset category code cannot exceed 255 characters.")]
        public string? CategoryCode { get; set; }

        [Required(ErrorMessage = "Asset Name is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Asset category name must be between 1 and 255 characters.")]
        public string? CategoryName { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        public Unit Unit { get; set; }
    }

    public class AssetCategoryUpdateDto
    {
        public string? CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public Unit? Unit { get; set; }
    }

    public class AssetCategoryQueryDto : BaseQueryDto
    {
        public string? CategoryCode { get; set; }
        public string? CategoryName { get; set; }
    }
}
