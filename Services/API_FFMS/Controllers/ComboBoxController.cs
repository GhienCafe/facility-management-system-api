using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers;

public class ComboBoxController : BaseController
{
    private readonly IComboBoxService _comboBoxService;
    public ComboBoxController(IComboBoxService service) 
    {
        _comboBoxService = service;
    }

    [HttpGet("room")]
    [SwaggerOperation("Get room combo box")]
    public async Task<ApiResponses<RoomComboBoxDto>> GetRoomBomboBoxs([FromQuery] ComboBoxQueryDto queryDto)
    {
        return await _comboBoxService.GetRoomBomboBoxs(queryDto);
    }
}
