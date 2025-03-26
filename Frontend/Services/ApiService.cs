using Shared;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Frontend.Services;

public class ApiService(
    IHttpClientFactory HttpClientFactory,
    ILogger<ApiService> logger)
{
    public async Task<string?> GetAccessToken(TokenRequest tokenRequest)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.PostAsync("api/Auth/token",
                new StringContent(JsonSerializer.Serialize(tokenRequest), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<TokenResponse>();

            return content!.AccessToken;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error getting access token.");

            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting access token.");

            return null;
        }
    }

    public async Task StartAirplane(int airplaneId)
    {
        var httpClient = HttpClientFactory.CreateClient("BackendAPI");

        await httpClient.PostAsync($"api/SignalR/airplanes/{airplaneId}/start", 
            new StringContent(string.Empty, Encoding.UTF8, "application/json"));

    }

    public async Task StopAirplane(int airplaneId)
    {
        var httpClient = HttpClientFactory.CreateClient("BackendAPI");

        await httpClient.PostAsync($"api/SignalR/airplanes/{airplaneId}/stop", 
            new StringContent(string.Empty, Encoding.UTF8, "application/json"));

    }
}
