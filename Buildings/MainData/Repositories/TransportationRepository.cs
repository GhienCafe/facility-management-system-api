using MainData.Entities;

namespace MainData.Repositories
{
    public interface ITransportationRepository 
    {
        Task<bool> IsConnected();
        Task<bool> InsertTransport(ActionRequest request, Guid? creatorId, DateTime? now = null);
    }
    public class TransportationRepository : ITransportationRepository
    {
        private readonly DatabaseContext _dbContext;

        public TransportationRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> InsertTransport(ActionRequest request, Guid? creatorId, DateTime? now = null)
        {
            await _dbContext.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                if (request != null)
                {
                    var newRequest = new ActionRequest
                    {
                        Id = request.Id,
                        RequestCode = request.RequestCode,
                        RequestDate = request.RequestDate,
                        CompletionDate = request.CompletionDate,
                        RequestType = request.RequestType,
                        RequestStatus = request.RequestStatus,
                        Description = request.Description,
                        Notes = request.Notes,
                        IsInternal = request.IsInternal,
                        AssignedTo = request.AssignedTo,
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        CreatorId = creatorId
                    };
                    await _dbContext.ActionRequests.AddAsync(newRequest);
                }
                if (request!.Transportations != null)
                {
                    foreach (var item in request.Transportations)
                    {
                        if (item != null)
                        {
                            item.CreatedAt = now.Value;
                            item.EditedAt = now.Value;
                            item.CreatorId = creatorId;
                        }
                    }
                    await _dbContext.Transportations.AddRangeAsync(request.Transportations);
                }
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
                return true;
            }
            catch (Exception)
            {
                await _dbContext.Database.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> IsConnected() {
            _dbContext.Database.BeginTransaction();

            _dbContext.Database.CommitTransaction();
            return true;
        }

        //public async Task<bool> InsertTransport(IEnumerable<ActionRequest> entities, Guid? creatorId, DateTime? now = null)
        //{
        //    await _dbContext.Database.BeginTransactionAsync();
        //    try
        //    {
        //        now ??= DateTime.UtcNow;
        //        entities = entities.Select(x =>
        //        {
        //            x!.CreatedAt = now.Value;
        //            x.EditedAt = now.Value;
        //            x.CreatorId = creatorId;
        //            return x;
        //        });
        //        await _dbContext.Transportations.AddRangeAsync(entities!);
        //        await _dbContext.SaveChangesAsync();
        //        await _dbContext.Database.CommitTransactionAsync();
        //    } catch (Exception ex)
        //    {
        //        await _dbContext.Database.RollbackTransactionAsync();
        //    }
         
        //}
    }
}
