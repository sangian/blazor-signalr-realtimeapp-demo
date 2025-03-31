using System.Text.Json.Serialization;

namespace Shared.ChatServer.ApiDtos;

public record GetRoomsResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("rooms")]
    public GetRoomsItem[]? Rooms { get; init; }
}

public record GetRoomsItem
{
    [JsonPropertyName("id")]
    public long? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("members")]
    public SortedSet<string>? Members { get; init; }
}
