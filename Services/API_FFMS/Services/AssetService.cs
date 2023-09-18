using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API_FFMS.Services;
public interface IAssetService : IBaseService
{
    Task<ApiResponses<AssetDto>> GetAllAssets(AssetQueryDto queryDto);
    Task<ApiResponse<AssetDetailDto>> GetAsset(Guid id);
    Task<ApiResponse> Create(AssetCreateDto createDto);
    Task<ApiResponse> Update(Guid id, AssetUpdateDto updateDto);
    Task<ApiResponse> Delete(Guid id);
}

public class AssetService : BaseService, IAssetService
{
    public AssetService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> Create(AssetCreateDto createDto)
    {
        var existingAssetCode = MainUnitOfWork.AssetRepository.GetQuery()
                                .Where(x => x.AssetCode.Trim().ToLower() == createDto.AssetCode.Trim().ToLower())
                                .SingleOrDefault();
        var exstingSerialNumber = MainUnitOfWork.AssetRepository.GetQuery()
                                .Where(x => x.SerialNumber.Trim().ToLower() == createDto.SerialNumber.Trim().ToLower())
                                .SingleOrDefault();

        if (existingAssetCode != null)
        {
            throw new ApiException("Asset Code is already in use, please choose a different name.", StatusCode.BAD_REQUEST);
        }

        if (exstingSerialNumber != null)
        {
            throw new ApiException("Serial Number is already in use, please choose a different name.", StatusCode.BAD_REQUEST);
        }

        var asset = createDto.ProjectTo<AssetCreateDto, Asset>();

        if (!await MainUnitOfWork.AssetRepository.InsertAsync(asset, AccountId, CurrentDate))
            throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Create successfully");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);

        if (existingAsset == null)
            throw new ApiException("Asset not found", StatusCode.NOT_FOUND);

        if (! await MainUnitOfWork.AssetRepository.DeleteAsync(existingAsset, AccountId, CurrentDate))
            throw new ApiException("Delete fail", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }

    public async Task<ApiResponses<AssetDto>> GetAllAssets(AssetQueryDto queryDto)
    {
        Expression<Func<Asset, bool>>[] conditions = new Expression<Func<Asset, bool>>[]
        {
            x => !x.DeletedAt.HasValue
        };

        if (string.IsNullOrEmpty(queryDto.AssetName) == false)
        {
            conditions = conditions.Append(x => x.AssetName.Trim().ToLower().Contains(queryDto.AssetName.Trim().ToLower())).ToArray();
        }

        if (string.IsNullOrEmpty(queryDto.AssetCode) == false)
        {
            conditions = conditions.Append(x => x.AssetCode.Trim().ToLower().Contains(queryDto.AssetCode.Trim().ToLower())).ToArray();
        }

        var response = await MainUnitOfWork.AssetRepository.FindResultAsync<AssetDto>(conditions, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);
        return ApiResponses<AssetDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<AssetDetailDto>> GetAsset(Guid id)
    {
        var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetDetailDto>(
            new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });
        if (existingAsset == null)
        {
            throw new ApiException("Not found this asset", StatusCode.NOT_FOUND);
        }

        existingAsset = await _mapperRepository.MapCreator(existingAsset);

        return ApiResponse<AssetDetailDto>.Success(existingAsset);
    }

    public async Task<ApiResponse> Update(Guid id, AssetUpdateDto updateDto)
    {
        var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(id);
        if (existingAsset == null)
        {
            throw new ApiException("Not found this asset", StatusCode.NOT_FOUND);
        }

        //var assetUpdate = updateDto.ProjectTo<AssetUpdateDto, Asset>();

        var assetUpdate = existingAsset;

        existingAsset.TypeId = updateDto.TypeId ?? existingAsset.TypeId;
        existingAsset.AssetName = updateDto.AssetName ?? existingAsset.AssetName;
        existingAsset.Status = updateDto.Status ?? existingAsset.Status;
        existingAsset.ManufacturingYear = updateDto.ManufacturingYear ?? existingAsset.ManufacturingYear;
        existingAsset.SerialNumber = updateDto.SerialNumber ?? existingAsset.SerialNumber;
        existingAsset.Quantity = updateDto.Quantity ?? existingAsset.Quantity;
        existingAsset.Description = updateDto.Description ?? existingAsset.Description;

        if (!await MainUnitOfWork.AssetRepository.UpdateAsync(assetUpdate, AccountId, CurrentDate))
        {
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }
}
