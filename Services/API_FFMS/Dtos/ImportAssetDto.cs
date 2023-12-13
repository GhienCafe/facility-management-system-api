using MainData.Entities;

namespace API_FFMS.Dtos;

public class ImportAssetDto
{
    public Guid Id { get; set; }
    public string AssetName { get; set; } = null!;
    public string AssetCode { get; set; } = null!;
    public string TypeCode { get; set; } = null!;
    public string ModelCode { get; set; } = null!;
    public string ManufacturingYear { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string Quantity { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string IsRented { get; set; } = null!;
    public string IsMovable { get; set; } = null!;
    public string RoomCode { get; set; } = null!;
}

public class ImportAssetToRoomDto
{
    public string? RoomName { get; set; }
    public string? AssetCode { get; set; }
    public string? Description { get; set; }
    public AssetStatus Status { get; set; }
    public DateTime? FromDate { get; set; }
    public double Quantity { get; set; }
}

public class ImportTransportError
{
    public int Row { get; set; }
    public string? ErrorMessage { get; set; }
    public List<AssetTransportDto>? AssetTransportImportDtos { get; set; }
}

public class AssetTransportImportDto
{
    public string? AssetName { get; set; }
    public string? AssetCode { get; set; }
    public string? AssetType { get; set; }
    public double Quantity { get; set; }
}