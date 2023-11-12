using System.ComponentModel.DataAnnotations;

namespace API_FFMS.Dtos;

public enum ImportTemplate
{
    [Display(Name = "AssetTemplate.xlsx")]
    AssetTemplate = 1,
}