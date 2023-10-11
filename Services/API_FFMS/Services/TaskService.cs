using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using DocumentFormat.OpenXml.Office2021.Excel.Pivot;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API_FFMS.Services;

public interface ITaskService : IBaseService
{
    Task<ApiResponses<TaskBaseDto>> GetTasks(TaskQueryDto queryDto);
}

public class TaskService : BaseService, ITaskService
{
    public TaskService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }


    public async Task<ApiResponses<TaskBaseDto>> GetTasks(TaskQueryDto queryDto)
    {
        // Retrieve tasks from the Transportation table
        var transportationTasks = MainUnitOfWork.TransportationRepository.GetQuery()
            .Where(t => !t!.DeletedAt.HasValue && t.AssignedTo == AccountId)
            .Select(t => new TaskBaseDto
            {
                Type = RequestType.Transportation,
                Id = t!.Id,
                CreatorId = t.CreatorId ?? Guid.Empty,
                EditorId = t.EditorId ?? Guid.Empty,
                CreatedAt = t.CreatedAt,
                EditedAt = t.EditedAt,
                ToRoomId = t.ToRoomId,
                Quantity = t.Quantity,
                AssignedTo = t.AssignedTo,
                CompletionDate = t.CompletionDate,
                Description = t.Description,
                IsInternal = t.IsInternal,
                Notes = t.Notes,
                RequestCode = t.RequestCode,
                RequestDate = t.RequestDate,
                Status = t.Status,
            });
        
        // Retrieve tasks from the Maintenance table
        var maintenanceTasks = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(t => !t!.DeletedAt.HasValue && t.AssignedTo == AccountId)
            .Select(t => new TaskBaseDto
            {
                Type = RequestType.Maintenance,
                Id = t!.Id,
                CreatorId = t.CreatorId ?? Guid.Empty,
                EditorId = t.EditorId ?? Guid.Empty,
                CreatedAt = t.CreatedAt,
                EditedAt = t.EditedAt,
                ToRoomId = null,
                Quantity = null,
                AssignedTo = t.AssignedTo,
                CompletionDate = t.CompletionDate,
                Description = t.Description,
                IsInternal = t.IsInternal,
                Notes = t.Notes,
                RequestCode = t.RequestCode,
                RequestDate = t.RequestDate,
                Status = t.Status,
            });

        // Concatenate the results from both tables
        var combinedTasks = transportationTasks.Union(maintenanceTasks);

        var totalCount = await combinedTasks.CountAsync();

        combinedTasks = combinedTasks.Skip(queryDto.Skip()).Take(queryDto.PageSize);
        
        var items = await combinedTasks.Select(x => new TaskBaseDto
        {
            Type = RequestType.Maintenance,
            Id = x.Id,
            CreatorId = x.CreatorId,
            EditorId = x.EditorId,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            AssignedTo = x.AssignedTo,
            CompletionDate = x.CompletionDate,
            Description = x.Description,
            Notes = x.Notes,
            IsInternal = x.IsInternal,
            RequestCode = x.RequestCode,
            RequestDate = x.RequestDate,
            Status = x.Status, 
            Quantity = x.Quantity,
            ToRoomId = x.ToRoomId
        }).ToListAsync();

        items.ForEach(x =>
        {
            x.StatusObj = x.Status?.GetValue();
            x.TypeObj = x.Type.GetValue();
        });
        
        items = await _mapperRepository.MapCreator(items);

        return ApiResponses<TaskBaseDto>.Success(
            items,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }
}