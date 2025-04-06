using System.Text;

namespace Frontend.Services;

public sealed class FlightService(IHttpClientFactory HttpClientFactory)
{
    public async Task StartAirplane(int airplaneId)
    {
        var httpClient = HttpClientFactory.CreateClient("BackendAPI");

        await httpClient.PostAsync($"api/Flight/airplanes/{airplaneId}/start", 
            new StringContent(string.Empty, Encoding.UTF8, "application/json"));

    }

    public async Task StopAirplane(int airplaneId)
    {
        var httpClient = HttpClientFactory.CreateClient("BackendAPI");

        await httpClient.PostAsync($"api/Flight/airplanes/{airplaneId}/stop", 
            new StringContent(string.Empty, Encoding.UTF8, "application/json"));

    }
}
