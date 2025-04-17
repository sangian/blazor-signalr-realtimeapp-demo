using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.TelemetryServer;

namespace Backend.LiveTelemetryServer;

[Authorize]
public sealed class LiveTelemetryHub(
    ILogger<LiveTelemetryHub> logger,
    LiveTelemetryChannelManager liveTelemetryChannelManager) : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("LiveTelemetryHub => Client connected with no user ID: {connectionId}", connectionId);
        }
        else
        {
            logger.LogInformation("LiveTelemetryHub => Client connected: {userId} {connectionId}", userId, connectionId);
        }

        var channel = Channel.CreateUnbounded<AirplaneTelemetry>();
        liveTelemetryChannelManager.AddChannel(connectionId, channel);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Identity?.Name;
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("LiveTelemetryHub => Client disconnected with no user ID: {connectionId}", connectionId);
        }
        else
        {
            logger.LogInformation("LiveTelemetryHub => Client disconnected: {userId} {connectionId}", userId, connectionId);
        }

        if (exception is not null)
        {
            logger.LogError(exception, "LiveTelemetryHub => Client {connectionId} disconnected with error: {Error}", connectionId, exception.Message);
        }

        liveTelemetryChannelManager.RemoveChannel(connectionId);

        return base.OnDisconnectedAsync(exception);
    }

    public ChannelReader<AirplaneTelemetry>? StreamTelemetry()
    {
        var connectionId = Context.ConnectionId;

        if (!liveTelemetryChannelManager.TryGetChannel(connectionId, out var channel))
        {
            logger.LogError("LiveTelemetryHub => Failed to get stream channel for connection ID: {connectionId}", connectionId);

            return null;
        }

        return channel?.Reader;
    }
}
