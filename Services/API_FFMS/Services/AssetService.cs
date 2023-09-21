﻿using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;
public interface IAssetService : IBaseService
{
    Task<ApiResponses<AssetDto>> GetAssets(AssetQueryDto queryDto);
    Task<ApiResponse<AssetDetailDto>> GetAsset(Guid id);
    Task<ApiResponse> Create(AssetCreateDto createDto);
    Task<ApiResponse> Update(Guid id, AssetUpdateDto updateDto);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponses<RoomAssetDto>> GetAssetsInRoom(Guid roomId, RoomAssetQueryDto queryDto);
}

public class AssetService : BaseService, IAssetService
{
    public AssetService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> Create(AssetCreateDto createDto)
    {
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

    public async Task<ApiResponses<AssetDto>> GetAssets(AssetQueryDto queryDto)
    {
        var assetDataSet = MainUnitOfWork.AssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        if (!string.IsNullOrEmpty(queryDto.AssetCode))
            assetDataSet = assetDataSet.Where(x => x!.AssetCode!.ToLower().Equals(queryDto.AssetCode));
        
        if (!string.IsNullOrEmpty(queryDto.AssetName))
            assetDataSet = assetDataSet.Where(x => x!.AssetName.ToLower().Contains(queryDto.AssetName!.Trim().ToLower()));
        
        if (!string.IsNullOrEmpty(queryDto.Description))
            assetDataSet = assetDataSet.Where(x => x!.Description!.ToLower().Contains(queryDto.Description!.Trim().ToLower()));
        
        if (!string.IsNullOrEmpty(queryDto.SerialNumber))
            assetDataSet = assetDataSet.Where(x => x!.SerialNumber!.ToLower().Contains(queryDto.SerialNumber!.Trim().ToLower()));
        
        if (queryDto.Status != null)
            assetDataSet = assetDataSet.Where(x => x!.Status == queryDto.Status);
        
        if (queryDto.IsMovable != null)
            assetDataSet = assetDataSet.Where(x => x!.IsMovable == queryDto.IsMovable);

        var totalCount = assetDataSet.Count();

        var assets = (await assetDataSet.Skip(queryDto.Skip())
            .Take(queryDto.PageSize)
            .ToListAsync())!.ProjectTo<Asset, AssetDto>();

        assets = await _mapperRepository.MapCreator(assets);

        return ApiResponses<AssetDto>.Success(
            assets,
            totalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
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
        
        existingAsset.TypeId = updateDto.TypeId ?? existingAsset.TypeId;
        existingAsset.AssetName = updateDto.AssetName ?? existingAsset.AssetName;
        existingAsset.Status = updateDto.Status ?? existingAsset.Status;
        existingAsset.ManufacturingYear = updateDto.ManufacturingYear ?? existingAsset.ManufacturingYear;
        existingAsset.SerialNumber = updateDto.SerialNumber ?? existingAsset.SerialNumber;
        existingAsset.Quantity = updateDto.Quantity ?? existingAsset.Quantity;
        existingAsset.Description = updateDto.Description ?? existingAsset.Description;
        existingAsset.AssetCode = updateDto.AssetCode ?? existingAsset.AssetCode;
        existingAsset.IsMovable = updateDto.IsMovable ?? existingAsset.IsMovable;
        existingAsset.LastMaintenanceTime = updateDto.LastMaintenanceTime ?? existingAsset.LastMaintenanceTime;

        if (!await MainUnitOfWork.AssetRepository.UpdateAsync(existingAsset, AccountId, CurrentDate))
            throw new ApiException("Can't not update", StatusCode.SERVER_ERROR);

        return ApiResponse.Success();
    }
    
     
     public async Task<ApiResponses<RoomAssetDto>> GetAssetsInRoom(Guid roomId, RoomAssetQueryDto queryDto)
     {
            // var room = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomDto>(new Expression<Func<Room, bool>>[]
            // {
            //     x => !x.DeletedAt.HasValue,
            //     x => x.Id == roomId
            // });
            //
            // if (room == null)
            //     throw new ApiException("Not find room", StatusCode.NOT_FOUND);
            //
            // var listAssetRoomQuery = MainUnitOfWork.RoomAssetRepository.GetQuery()
            //     .Where(x => !x!.DeletedAt.HasValue && x.RoomId == roomId);
            //
            // if (queryDto.IsInCurrent is true)
            // {
            //     listAssetRoomQuery = listAssetRoomQuery.Where(x => x!.ToDate == null);
            // }
            // else if (queryDto.ToDate != null)
            // {
            //     listAssetRoomQuery = listAssetRoomQuery.Where(x => x!.ToDate <= queryDto.ToDate);
            // }
            //
            // if(queryDto.FromDate != null)
            //     listAssetRoomQuery = listAssetRoomQuery.Where(x => x!.FromDate >= queryDto.FromDate);
            //
            // var listAssetIds = listAssetRoomQuery.Select(x => x!.AssetId).ToList();
            //
            // var assetQuery = MainUnitOfWork.AssetRepository.GetQuery()
            //     .Where(x => !x!.DeletedAt.HasValue && listAssetIds.Contains(x.Id));
            //
            // if (!string.IsNullOrEmpty(queryDto.AssetCode))
            //     assetQuery = assetQuery.Where(x => x!.AssetCode!.ToLower().Equals(queryDto.AssetCode.Trim().ToLower()));
            //
            // if (queryDto.Status != null)
            //     assetQuery = assetQuery.Where(x => x!.Status == queryDto.Status);
            //
            // if (!string.IsNullOrEmpty(queryDto.AssetName))
            //     assetQuery = assetQuery.Where(x => x!.AssetName!.ToLower().Contains(queryDto.AssetName.Trim().ToLower()));
            //
            //
            // var totalCount = assetQuery.Count();
            //
            // var assets = (await assetQuery.Skip(queryDto.Skip())
            //     .Take(queryDto.PageSize)
            //     .ToListAsync())!.ProjectTo<Asset, AssetDto>();
            //
            // assets = await _mapperRepository.MapCreator(assets);
            //
            // return ApiResponses<RoomAssetDto>.Success(
            //     assets,
            //     totalCount,
            //     queryDto.PageSize,
            //     queryDto.Skip(),
            //     (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
            // );
            throw new ApiException("Not implement");
     }
}
