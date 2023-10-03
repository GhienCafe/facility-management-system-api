﻿using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class RoomAssetController : BaseController
    {
        private readonly IRoomAssetService _service;

        public RoomAssetController(IRoomAssetService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation("Tracking asset used in room")]
        public async Task<ApiResponses<AssetTrackingDto>> AssetUsedTracking([FromQuery] RoomTrackingQueryDto queryDto, Guid id)
        {
            return await _service.AssetUsedTracking(id, queryDto);
        }

        //[HttpGet("track-room")]
        //[SwaggerOperation("Tracking room")]
        //public async Task<ApiResponses<AssetTrackingDto>> RoomTracking([FromQuery] RoomTrackingQueryDto queryDto)
        //{
        //    return await _service.AssetUsedTracking(queryDto);
        //}

        //[HttpGet("internal")]
        //public async Task<ApiResponse> AddToWareHouse()
        //{
        //    return await _service.AddListRoomAsset();
        //}
    }
}
