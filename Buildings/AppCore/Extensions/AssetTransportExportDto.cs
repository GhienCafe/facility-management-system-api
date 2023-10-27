using System.ComponentModel.DataAnnotations;

namespace AppCore.Extensions
{
    public class AssetTransportExportDto
    {
        [Display(Name = "Tên thiết bị")]
        public string? AssetName { get; set; }
    }
}