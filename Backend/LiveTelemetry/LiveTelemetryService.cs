using Microsoft.AspNetCore.SignalR;
using Shared;
using Shared.TelemetryServer;

namespace Backend.LiveTelemetry;

public sealed class LiveTelemetryService(
    IHubContext<LiveTelemetryHub> liveTelemetryHubContext)
{
    public async Task NotifyAirplaneStartResponse(GenericResponse genericResponse)
    {
        await liveTelemetryHubContext.Clients.All
            .SendAsync(Constants.CLIENT_NOTIFY_AIRPLANE_START_RESPONSE, genericResponse, default);
    }

    public async Task NotifyAirplaneStopResponse(GenericResponse genericResponse)
    {
        await liveTelemetryHubContext.Clients.All
            .SendAsync(Constants.CLIENT_NOTIFY_AIRPLANE_STOP_RESPONSE, genericResponse, default);
    }
}
