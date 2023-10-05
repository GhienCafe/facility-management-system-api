using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;

public class RepairationDto : BaseRequestDto {
    public AssetBaseDto? Asset { get; set; }
    public UserDto? User { get; set; }
}