using API_FFMS.Dtos;
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
                    RequestDate = createDto.RequestDate
                };
                
                _dbContext.ActionRequests.Add(request);
                await _dbContext.SaveChangesAsync();

                if (createDto.Transportations != null && createDto.Transportations.Any())
                {
                    var transports = createDto.Transportations.Select(x => new Transportation
                    {
                        Quantity = x.Quantity,
                        RequestId = requestId,
                        ToRoomId = x.ToRoomId,
                        AssetId = x.AssetId
                    });
                
                    _dbContext.Transportations.AddRange(transports);
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
