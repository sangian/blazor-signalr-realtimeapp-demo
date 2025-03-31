using Backend.LiveTelemetry;
using Microsoft.AspNetCore.SignalR;
using Shared;
using Shared.TelemetryServer;

namespace Backend.TelemetryServer;

public sealed class TelemetryServerService(
    ILogger<TelemetryServerService> logger,
    IHubContext<TelemetryServerHub> telemetryServerHubContext,
    LiveTelemetryService liveTelemetryService,
    TelemetryServerUserManager telemetryServerUserManager)
{
    public string GetUserId(int airplaneId)
    {
        return $"airplane-{airplaneId}";
    }

    public async Task StartRequest(int airplaneId)
    {
        logger.LogInformation($"TelemetryService => Starting Airplane {airplaneId}...");

        var userId = GetUserId(airplaneId);

        if (telemetryServerUserManager.IsUserConnected(userId))
        {
            await telemetryServerHubContext.Clients
                .User(userId)
                .SendAsync(Constants.CLIENT_START_REQUEST);
        }
        else
        {
            logger.LogInformation($"TelemetryService => Airplane {airplaneId} is offline.");

            await liveTelemetryService.NotifyAirplaneStartResponse(new GenericResponse
            {
                AirplaneId = airplaneId,
                Success = false,
                ErrorMessage = $"Airplane {airplaneId} is offline."
            });
        }
    }

    public async Task StopRequest(int airplaneId)
    {
        logger.LogInformation($"TelemetryService => Stopping Airplane {airplaneId}...");

        var userId = GetUserId(airplaneId);

        if (telemetryServerUserManager.IsUserConnected(userId))
        {
            await telemetryServerHubContext.Clients
                .User(userId)
                .SendAsync(Constants.CLIENT_STOP_REQUEST);
        }
        else
        {
            logger.LogInformation($"TelemetryService => Airplane {airplaneId} is offline.");

            await liveTelemetryService.NotifyAirplaneStopResponse(new GenericResponse
            {
                AirplaneId = airplaneId,
                Success = false,
                ErrorMessage = $"Airplane {airplaneId} is offline."
            });
        }
    }

    public async Task AddToGroup(string userId, string groupName)
    {
        var connectionIds = telemetryServerUserManager.GetUserConnections(userId);
        foreach (var connectionId in connectionIds)
        {
            await telemetryServerHubContext.Groups.AddToGroupAsync(connectionId, groupName);
        }
    }

    public async Task RemoveFromGroup(string userId, string groupName)
    {
        var connectionIds = telemetryServerUserManager.GetUserConnections(userId);
        foreach (var connectionId in connectionIds)
        {
            await telemetryServerHubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }
    }

    public async Task PingConnection(string connectionId)
    {
        await telemetryServerHubContext.Clients
            .Client(connectionId)
            .SendAsync(Constants.PING);
    }

    public async Task PingUser(string userId)
    {
        await telemetryServerHubContext.Clients
            .User(userId)
            .SendAsync(Constants.PING);
    }

    public async Task PingGroup(string groupName)
    {
        await telemetryServerHubContext.Clients
            .Group(groupName)
            .SendAsync(Constants.PING);
    }

    public async Task PingAll()
    {
        await telemetryServerHubContext.Clients
            .All
            .SendAsync(Constants.PING);
    }
}
