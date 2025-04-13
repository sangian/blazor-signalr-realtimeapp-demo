using System.Text.Json.Serialization;

namespace Shared.ChatServer.ApiDtos;

public record CreateRoomResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }

    [JsonPropertyName("id")]
    public long? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("members")]
    public SortedSet<string>? Members { get; init; }
}
