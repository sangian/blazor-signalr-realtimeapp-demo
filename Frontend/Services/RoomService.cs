using Shared.ChatServer.ApiDtos;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Frontend.Services;

public sealed class RoomService(
    IHttpClientFactory HttpClientFactory,
    ILogger<RoomService> logger)
{
    public async Task<CreateRoomResponse?> CreateRoom(CreateRoomRequest request)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.PostAsync("api/Rooms",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<CreateRoomResponse>();

            return content;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error creating room.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating room.");
            return null;
        }
    }

    public async Task<AddRoomMemberResponse?> AddMember(long roomId, AddRoomMemberRequest request)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.PostAsync($"api/chat/Rooms/{roomId}/members",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<AddRoomMemberResponse>();

            return content;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error adding member to room.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding member to room.");
            return null;
        }
    }

    public async Task<RemoveRoomMemberResponse?> RemoveMember(long roomId, string userId)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.DeleteAsync($"api/Rooms/{roomId}/members/{userId}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<RemoveRoomMemberResponse>();

            return content!;
        }
        catch (HttpRequestException)
        {
            return null!;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error removing member from room.");
            return null!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing member from room.");
            return null!;
        }
    }

    public async Task<DeleteRoomResponse?> DeleteRoom(long roomId)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.DeleteAsync($"api/Rooms/{roomId}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<DeleteRoomResponse>();

            return content;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error deleting room.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting room.");
            return null;
        }
    }

    public async Task<GetRoomsItem[]> GetRooms()
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("BackendAPI");

            var response = await httpClient.GetAsync("api/Rooms");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<GetRoomsResponse>();

            return content?.Rooms ?? [];
        }
        catch (HttpRequestException)
        {
            return [];
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Error getting rooms.");

            return [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting rooms.");

            return [];
        }
    }
}
