using System.ComponentModel.DataAnnotations;
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class ReplaceDto : BaseRequestDto {
    public Guid NewAssetId { get; set; }
    public AssetBaseDto? Asset { get; set; }
    public AssetBaseDto? NewAsset { get; set; }
    public UserDto? User { get; set; }
}