using Frontend.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Services
{
    public sealed class LiveTelemetryService(
        ILogger<LiveTelemetryService> logger,
        IConfiguration configuration,
        AuthenticationStateProvider authStateProvider) : IDisposable
    {
        private HubConnection? hubConnection;

        public async Task<HubConnection?> InitalizeHubConnection()
        {
            try
            {
                if (hubConnection is null)
                {

                    var hubUrl = configuration["SignalROptions:HubUrl"];
                    var accessToken = await ((CustomAuthenticationStateProvider)authStateProvider).GetAuthToken();

                    hubConnection = new HubConnectionBuilder()
                        .WithUrl(new Uri(hubUrl!), o => o.AccessTokenProvider = () => Task.FromResult(accessToken))
                        .WithAutomaticReconnect()
                        .Build();
                }

                return hubConnection;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cannot connect to SignalR hub.");

                throw;
            }
        }

        public void Dispose()
        {
            if (hubConnection is not null)
            {
                hubConnection.DisposeAsync().AsTask().Wait();
            }

            GC.SuppressFinalize(this);
        }
    }
}
