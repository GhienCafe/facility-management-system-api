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
        public bool? IsIdentified { get; set; }
        public Unit? Unit { get; set; }
        public EnumValue? UnitObj { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? CategoryId { get; set; }
    }

    public class AssetTypeSheetDto : BaseDto
    {
        public string? TypeCode { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public Unit? Unit { get; set; }
        public string? ImageUrl { get; set; }
        public EnumValue? UnitObj { get; set; }
        public Guid? CategoryId { get; set; }
    }

    public class AssetTypeDetailDto : BaseDto
    {
        public string TypeCode { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public string? Description { get; set; }
        public Unit? Unit { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? CategoryId { get; set; }
        public EnumValue? UnitObj { get; set; }
    }

    public class AssetTypeCreateDto
    {
        [Required(ErrorMessage = "Mã loại trang thiết bị không được để trống")]
        public string TypeCode { get; set; } = null!;
        [Required(ErrorMessage = "Tên loại trang thiết bị không được để trống")]
        public string TypeName { get; set; } = null!;
        public string? Description { get; set; }
        [Required(ErrorMessage = "Đơn vị tính không được trống")]
        public Unit Unit { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsIdentified { get; set; } = false;
    }

    public class AssetTypeUpdateDto
    {
        public string? TypeCode { get; set; } 
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public Unit? Unit { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsIdentified { get; set; } 
    }

    public class AssetTypeQueryDto : BaseQueryDto
    {
        public Unit? Unit { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
