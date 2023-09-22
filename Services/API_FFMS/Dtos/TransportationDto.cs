
using AppCore.Models;
using MainData.Entities;

namespace API_FFMS.Dtos;
public class TransportCreateDto
{
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    public DateTime? ActualDate { get; set; }
    public TransportationStatus Status { get; set; }
    public Guid AssignedTo { get; set; }
    public List<TransportAssetCreateDto>? Assets { get; set; }
}

public class TransportAssetCreateDto
{
    public Guid AssetId { get; set; }
    public Guid SourceLocation { get; set; }
    public Guid DestinationLocation { get; set; }
    public string? Description { get; set; }
}

public class TransportUpdateDto
{
    public DateTime? ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? Description { get; set; }
    public TransportationStatus? Status { get; set; }
    public Guid? AssignedTo { get; set; }
}

public class TransportDetailUpdateDto
{
    public Guid? AssetId { get; set; }
    public Guid? SourceLocation { get; set; }
    public Guid? DestinationLocation { get; set; }
    public bool IsDone { get; set; }
    public string? Description { get; set; }
}

public class TransportationDto
{
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string? Description { get; set; }
    public List<AssetTransportDto>? IncludedAssets { get; set; }
}

public class AssetTransportDto
{
    public string? AssetCode { get; set; }
    public string? AssetName { get; set; }
}

public class TransportQueryDto : BaseQueryDto
{
    public Guid? DestinationLocation { get; set; }
    public string? AssetName { get; set; }
    public string? AssetCode { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public TransportationStatus Status { get; set; }
}

public class TransportDto
{
    public Guid? AssetId { get; set; }
    public Guid? SourceLocation { get; set; }
    public Guid? DestinationLocation { get; set; }
    public bool IsDone { get; set; }
    public string? Description { get; set; }
    public List<AssetDto>? Asset { get; set; }

}