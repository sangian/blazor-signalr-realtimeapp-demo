using Shared.ChatServer.ApiDtos;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Frontend.Services;

public sealed class MessageService(
    IHttpClientFactory HttpClientFactory,
    ILogger<MessageService> logger)
{
    public async Task<SendMessageResponse?> SendMessage(long roomId, SendMessageRequest request)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.PostAsync($"api/Rooms/{roomId}/Messages",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<SendMessageResponse>();

            return content;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error sending message.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending message.");
            return null;
        }
    }

    public async Task<bool> MarkMessageAsDelivered(long roomId, long messageId)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.PatchAsync($"api/Rooms/{roomId}/Messages/{messageId}/delivered",
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error marking message as delivered.");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error marking message as delivered.");
            return false;
        }
    }

    public async Task<bool> MarkMessagesAsRead(long roomId)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.PatchAsync($"api/Rooms/{roomId}/Messages/read",
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error marking message as read.");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error marking message as read.");
            return false;
        }
    }
}
