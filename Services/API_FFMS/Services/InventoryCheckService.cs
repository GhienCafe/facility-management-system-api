using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API_FFMS.Services;
public interface IInventoryCheckService : IBaseService
{
    Task<ApiResponse> Create(InventoryCheckCreateDto createDto);
    Task<ApiResponse> Delete(Guid id);
}


public class InventoryCheckService : BaseService, IInventoryCheckService
{
    public InventoryCheckService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                                 IMapperRepository mapperRepository)
                                 : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> Create(InventoryCheckCreateDto createDto)
    {
        var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDto.AssetIds.Contains(x.Id)
                }, null);

        var inventoryCheck = new InventoryCheck
        {
            RequestCode = GenerateRequestCode(),
            RequestDate = CurrentDate,
            Description = createDto.Description,
            Notes = createDto.Notes,
            Priority = createDto.Priority,
            IsInternal = createDto.IsInternal,
            AssignedTo = createDto.AssignedTo
        };

        throw new NotImplementedException();
    }

    public Task<ApiResponse> Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public string GenerateRequestCode()
    {
        var requests = MainUnitOfWork.InventoryCheckRepository.GetQueryAll().ToList();

        var numbers = new List<int>();
        foreach (var t in requests)
        {
            int.TryParse(t!.RequestCode[3..], out int lastNumber);
            numbers.Add(lastNumber);
        }

        string newRequestCode = "IVC1";

        if (requests.Any())
        {
            var lastCode = numbers.AsQueryable().OrderDescending().FirstOrDefault();
            if (requests.Any(x => x!.RequestCode.StartsWith("IVC")))
            {
                newRequestCode = $"IVC{lastCode + 1}";
            }
        }
        return newRequestCode;
    }
}
