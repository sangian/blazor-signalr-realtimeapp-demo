using Backend.TelemetryServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class FlightController(
    TelemetryServerService telemetryServerService,
    TelemetryServerUserManager telemetryServerUserManager) : ControllerBase
{
    [HttpPost("airplanes/{airplaneId}/start")]
    public async Task<IActionResult> StartAirplane(int airplaneId)
    {
        await telemetryServerService.StartRequest(airplaneId);

        return Ok();
    }

    [HttpPost("airplanes/{airplaneId}/stop")]
    public async Task<IActionResult> StopAirplane(int airplaneId)
    {
        await telemetryServerService.StopRequest(airplaneId);

        return Ok();
    }

    [HttpPost("airplanes/{airplaneId}/groups/{groupName}")]
    public async Task<IActionResult> AddToGroup(int airplaneId, string groupName)
    {
        var userId = telemetryServerService.GetUserId(airplaneId);
        await telemetryServerService.AddToGroup(userId, groupName);

        return Ok();
    }

    [HttpDelete("airplanes/{airplaneId}/groups/{groupName}")]
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


    [HttpPost("connections/{connectionId}/ping")]
    public async Task<IActionResult> PingConnection(string connectionId)
    {
        await telemetryServerService.PingConnection(connectionId);

        return Ok();
    }

    [HttpPost("connections/ping")]
    public async Task<IActionResult> PingAll()
    {
        await telemetryServerService.PingAll();

        return Ok();
    }

    [HttpPost("users/{userId}/ping")]
    public async Task<IActionResult> PingUser(string userId)
    {
        await telemetryServerService.PingUser(userId);

        return Ok();
    }

    [HttpPost("groups/{groupName}/ping")]
    public async Task<IActionResult> PingGroup(string groupName)
    {
        await telemetryServerService.PingGroup(groupName);

        return Ok();
    }
}
