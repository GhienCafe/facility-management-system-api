using AppCore.Models;

namespace API_FFMS.Dtos;

public class ComboBoxDto
{
}

public class ComboBoxQueryDto {
    public string? Keyword { get; set; }
}


public class RoomComboBoxDto
{
    public Guid Id { get; set; }
    public string? RoomName { get; set; }
    public string? RoomCode { get; set; }
    public Guid? StatusId { get; set; }
    public RoomStatusComboBoxDto? Status { get; set; }
}

public class RoomStatusComboBoxDto
{
    public Guid Id { get; set; }
    public string? StatusName { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class AssetComboBoxDto
{
    public Guid Id { get; set; }
    public string? AssetName { get; set; }
    public string? AssetCode { get; set; }

}