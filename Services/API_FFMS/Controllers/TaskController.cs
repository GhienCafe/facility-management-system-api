using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
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
    public async Task<ApiResponses<TaskDto>> GetTasks([FromQuery] TaskQueryDto queryDto)
    {
        return await _taskService.GetTasks(queryDto);
    }
}