using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API_FFMS.Services;

public interface IBrandService : IBaseService
{
    Task<ApiResponse> Create(BrandCreateDto createDto);
    Task<ApiResponse<BrandDto>> GetBrand(Guid id);
    public Task<ApiResponse> Update(Guid id, BrandUpdateDto updateDto);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponse> DeleteBrands(DeleteMutilDto deleteDto);
    Task<ApiResponses<BrandDto>> GetBrands(BrandQueryDto queryDto);
    Task<ApiResponses<BrandDto>> GetBrands();
}

public class BrandService : BaseService, IBrandService
{
    public BrandService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse> Create(BrandCreateDto createDto)
    {
        var brand = createDto.ProjectTo<BrandCreateDto, Brand>();
        if (!await MainUnitOfWork.BrandRepository.InsertAsync(brand, AccountId, CurrentDate))
        {
            throw new ApiException("Thêm mới thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Created("Thêm mới thành công");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingBrand = await MainUnitOfWork.BrandRepository.FindOneAsync(id);
        if (existingBrand == null)
        {
            throw new ApiException("Không tìm thấy nhãn hiệu", StatusCode.NOT_FOUND);
        }

        if (!await MainUnitOfWork.BrandRepository.DeleteAsync(existingBrand, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse> DeleteBrands(DeleteMutilDto deleteDto)
    {
        var brandDeleteds = await MainUnitOfWork.BrandRepository.FindAsync(
            new Expression<Func<Brand, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => deleteDto.ListId!.Contains(x.Id)
            }, null);

        if (!await MainUnitOfWork.BrandRepository.DeleteAsync(brandDeleteds, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }

    public async Task<ApiResponse<BrandDto>> GetBrand(Guid id)
    {
        var brand = await MainUnitOfWork.BrandRepository.FindOneAsync<BrandDto>(
            new Expression<Func<Brand, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

        if (brand == null)
        {
            throw new ApiException("Không tìm thấy nhãn hiệu", StatusCode.NOT_FOUND);
        }

        brand = await _mapperRepository.MapCreator(brand);

        return ApiResponse<BrandDto>.Success(brand);
    }

    public async Task<ApiResponses<BrandDto>> GetBrands(BrandQueryDto queryDto)
    {
        var response = await MainUnitOfWork.BrandRepository.FindResultAsync<BrandDto>(
                new Expression<Func<Brand, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => string.IsNullOrEmpty(queryDto.BrandName) ||
                         x.BrandName.ToLower().Contains(queryDto.BrandName.Trim().ToLower())
                }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

        response.Items = await _mapperRepository.MapCreator(response.Items.ToList());

        return ApiResponses<BrandDto>.Success(
            response.Items,
            response.TotalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(response.TotalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponses<BrandDto>> GetBrands()
    {
        var response = MainUnitOfWork.BrandRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue);
        var brands = response.Select(x => new BrandDto
        {
            Id = x!.Id,
            BrandName = x.BrandName,
            Description = x.Description,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            EditorId = x.EditorId ?? Guid.Empty,
            CreatorId = x.CreatorId ?? Guid.Empty
        }).ToList();

        brands = await _mapperRepository.MapCreator(brands);

        return ApiResponses<BrandDto>.Success(brands);
    }

    public async Task<ApiResponse> Update(Guid id, BrandUpdateDto updateDto)
    {
        var existingBrand = await MainUnitOfWork.BrandRepository.FindOneAsync(id);
        if (existingBrand == null)
        {
            throw new ApiException("Không tìm thấy nhãn hiệu", StatusCode.NOT_FOUND);
        }

        existingBrand.BrandName = updateDto.BrandName ?? existingBrand.BrandName;
        existingBrand.Description = updateDto.Description ?? existingBrand.Description;

        if (!await MainUnitOfWork.BrandRepository.UpdateAsync(existingBrand, AccountId, CurrentDate))
        {
            throw new ApiException("Không thể cập nhật", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success();
    }
}
