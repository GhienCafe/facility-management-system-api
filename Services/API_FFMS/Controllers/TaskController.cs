using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using MainData.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class TaskController : BaseController
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    [SwaggerOperation("Get list tasks")]
    public async Task<ApiResponses<TaskBaseDto>> GetTasks([FromQuery] TaskQueryDto queryDto)
    {
        return await _taskService.GetTasks(queryDto);
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Get detail task")]
    public async Task<ApiResponse<TaskDetailDto>> GetTask(Guid id)
    {
        return await _taskService.GetTaskDetail(id);
    }

    [HttpPut]
    [SwaggerOperation("Send task report")]
    public async Task<ApiResponse> UpdateTaskStatus([FromBody] ReportCreateDto createDto, RequestStatus status)
    {
        return await _taskService.UpdateTaskStatus(createDto, status);
    }
}