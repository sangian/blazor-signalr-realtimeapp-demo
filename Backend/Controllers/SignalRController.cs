﻿using Backend.TelemetryServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class SignalRController(
    TelemetryServerService telemetryServerService,
    TelemetryServerUserManager telemetryServerUserManager) : ControllerBase
{
    [HttpPost("airplanes/{airplaneId}/start", Name = nameof(StartAirplane))]
    public async Task<IActionResult> StartAirplane(int airplaneId)
    {
        await telemetryServerService.StartRequest(airplaneId);

        return Ok();
    }

    [HttpPost("airplanes/{airplaneId}/stop", Name = nameof(StopAirplane))]
    public async Task<IActionResult> StopAirplane(int airplaneId)
    {
        await telemetryServerService.StopRequest(airplaneId);

        return Ok();
    }

    [HttpPost("airplanes/{airplaneId}/groups/{groupName}", Name = nameof(AddToGroup))]
    public async Task<IActionResult> AddToGroup(int airplaneId, string groupName)
    {
        var userId = telemetryServerService.GetUserId(airplaneId);
        await telemetryServerService.AddToGroup(userId, groupName);

        return Ok();
    }

    [HttpDelete("airplanes/{airplaneId}/groups/{groupName}", Name = nameof(RemoveFromGroup))]
    public async Task<IActionResult> RemoveFromGroup(int airplaneId, string groupName)
    {
        var userId = telemetryServerService.GetUserId(airplaneId);
        await telemetryServerService.RemoveFromGroup(userId, groupName);

        return Ok();
    }

    [HttpGet("airplanes", Name = nameof(GetConnectedUsers))]
    public IActionResult GetConnectedUsers()
    {
        return Ok(telemetryServerUserManager.GetConnectedUsers());
    }


    [HttpPost("connections/{connectionId}/ping", Name = nameof(PingConnection))]
    public async Task<IActionResult> PingConnection(string connectionId)
    {
        await telemetryServerService.PingConnection(connectionId);

        return Ok();
    }

    [HttpPost("connections/ping", Name = nameof(PingAll))]
    public async Task<IActionResult> PingAll()
    {
        await telemetryServerService.PingAll();

        return Ok();
    }

    [HttpPost("users/{userId}/ping", Name = nameof(PingUser))]
    public async Task<IActionResult> PingUser(string userId)
    {
        await telemetryServerService.PingUser(userId);

        return Ok();
    }

    [HttpPost("groups/{groupName}/ping", Name = nameof(PingGroup))]
    public async Task<IActionResult> PingGroup(string groupName)
    {
        await telemetryServerService.PingGroup(groupName);

        return Ok();
    }
}
