﻿using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using MainData.Entities;
using MainData.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class MissionController : BaseController
{
    private readonly IMissionService _missionService;

    public MissionController(IMissionService missionService)
    {
        _missionService = missionService;
    }

    [HttpGet]
    [SwaggerOperation("Get list task for technical")]
    public async Task<ApiResponses<MissionDto>> GetListTask([FromQuery]QueryMissionDto queryDto)
    {
        return await _missionService.GetListTask(queryDto);
    }
}