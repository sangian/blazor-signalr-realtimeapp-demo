using Frontend.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Services;

public sealed class ChatServerService(
    ILogger<ChatServerService> logger,
    IConfiguration configuration,
    AuthenticationStateProvider authStateProvider) : IDisposable
{
    private HubConnection? hubConnection;

    public async Task<HubConnection?> GetHubConnection()
    {
        try
        {
            if (hubConnection is null)
            {

                var hubUrl = configuration["SignalROptions:ChatServerHubUrl"];
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
            logger.LogError(ex, "Cannot connect to Chat Server hub.");

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
