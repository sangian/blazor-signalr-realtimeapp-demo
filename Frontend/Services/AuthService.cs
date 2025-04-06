using Shared.Authentication;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;

namespace Frontend.Services;

public sealed class AuthService(
    IHttpClientFactory HttpClientFactory,
    ILogger<FlightService> logger)
{
    public async Task<string?> Login(TokenRequest tokenRequest)
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
}
