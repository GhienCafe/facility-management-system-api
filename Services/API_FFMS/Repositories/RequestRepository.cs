using API_FFMS.Dtos;
using AppCore.Extensions;
using MainData;
using MainData.Entities;

namespace API_FFMS.Repositories
{
    public interface IRequestRepository
    {
        Task<bool> InsertRequest(ActionRequestCreateDto request, Guid? creatorId, DateTime? now = null);
    }
    public class RequestRepository : IRequestRepository
    {
        private readonly DatabaseContext _dbContext;

        public RequestRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> InsertRequest(ActionRequestCreateDto createDto, Guid? creatorId, DateTime? now = null)
        {
            await _dbContext.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                var requestId = Guid.NewGuid();
                var request = new ActionRequest
                {
                    Id = requestId,
                    Description = createDto.Description,
                    Notes = createDto.Notes,
                    RequestType = createDto.RequestType,
                    RequestStatus = createDto.RequestStatus,
                    AssignedTo = createDto.AssignedTo,
                    IsInternal = createDto.IsInternal,
                    RequestCode = createDto.RequestCode,
                    RequestDate = createDto.RequestDate,
                    CreatedAt = now.Value,
                    CreatorId = creatorId,
                };
                
                _dbContext.ActionRequests.Add(request);
                await _dbContext.SaveChangesAsync();

                if (createDto.Transportations != null && createDto.Transportations.Any() && request.RequestType == RequestType.Transportation)
                {
                    var transports = createDto.Transportations.Select(x => new Transportation
                    {
                        Quantity = x.Quantity,
                        RequestId = requestId,
                        ToRoomId = x.ToRoomId,
                        AssetId = x.AssetId,
                        CreatedAt = now.Value,
                        CreatorId = creatorId
                    });
                
                    _dbContext.Transportations.AddRange(transports);
                    await _dbContext.SaveChangesAsync();
                }
                
                if (createDto.Maintenances != null && createDto.Maintenances.Any() && request.RequestType == RequestType.Maintenance)
                {
                    var maintenance = createDto.Maintenances.Select(x => new Maintenance
                    {
                        RequestId = requestId,
                        AssetId = x.AssetId,
                        Notes = x.Note,
                        CreatedAt = now.Value,
                        CreatorId = creatorId
                    });
                
                    _dbContext.Maintenances.AddRange(maintenance);
                    await _dbContext.SaveChangesAsync();
                }
                
                if (createDto.Repairations != null && createDto.Repairations.Any() && request.RequestType == RequestType.Repairation)
                {
                    var repairations = createDto.Repairations.Select(x => new Repairation
                    {
                        RequestId = requestId,
                        AssetId = x.AssetId,
                        Notes = x.Note,
                        Description = x.Description,
                        CreatedAt = now.Value,
                        CreatorId = creatorId
                    });
                
                    _dbContext.Repairations.AddRange(repairations);
                    await _dbContext.SaveChangesAsync();
                }
                
                if (createDto.Replacements != null && createDto.Replacements.Any() && request.RequestType == RequestType.Replacement)
                {
                    var replacements = createDto.Replacements.Select(x => new Replacement
                    {
                        RequestId = requestId,
                        AssetId = x.AssetId,
                        NewAssetId = x.NewAssetId,
                        CreatedAt = now.Value,
                        CreatorId = creatorId
                    });
                
                    _dbContext.Replacements.AddRange(replacements);
                    await _dbContext.SaveChangesAsync();
                }

                await _dbContext.Database.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _dbContext.Database.RollbackTransactionAsync();
                return false;
            }
        }
    }
}
