using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface ICategoryService : IBaseService
{
    Task<ApiResponses<CategoryDto>> GetCategories(CategoryQueryDto queryDto);
}

public class CategoryService : BaseService, ICategoryService
{
    public CategoryService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }


    public async Task<ApiResponses<CategoryDto>> GetCategories(CategoryQueryDto queryDto)
    {
        var categoriesDataset = MainUnitOfWork.CategoryRepository.GetQuery().Where(x => !x!.DeletedAt.HasValue);
        
        if (!string.IsNullOrEmpty(queryDto.CategoryName))
        {
            categoriesDataset = categoriesDataset.Where(x =>
                x!.CategoryName.ToLower().Contains(queryDto.CategoryName!.Trim().ToLower()));
        }
        
        if (!string.IsNullOrEmpty(queryDto.Description))
        {
            categoriesDataset = categoriesDataset.Where(x =>
                x!.Description!.ToLower().Contains(queryDto.Description!.Trim().ToLower()));
        }
        
        if (queryDto.TeamId != null)
        {
            categoriesDataset = categoriesDataset.Where(x => queryDto.TeamId == null || x!.TeamId == queryDto.TeamId);
        }

        var joinTables = from category in categoriesDataset
            join team in MainUnitOfWork.TeamRepository.GetQuery() on category.TeamId equals team.Id
                into teamGroup
            from team in teamGroup.DefaultIfEmpty()
            select new
            {
                Category = category,
                Team = team
            };

        var totalCount = joinTables.Count();
        
        joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var categories = await joinTables.Select(
            x => new CategoryDto
            {
                CategoryName = x.Category.CategoryName,
                Description = x.Category.Description,
                CreatorId = x.Category.CreatorId ?? Guid.Empty,
                Id = x.Category.Id,
                CreatedAt = x.Category.CreatedAt,
                EditedAt = x.Category.EditedAt,
                EditorId = x.Category.EditorId ?? Guid.Empty,
                TeamId = x.Category.TeamId,
                Team = x.Team.ProjectTo<Team, TeamDto>()
            }).ToListAsync();

        categories = await _mapperRepository.MapCreator(categories);
        
        return ApiResponses<CategoryDto>.Success(
            categories,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }
}