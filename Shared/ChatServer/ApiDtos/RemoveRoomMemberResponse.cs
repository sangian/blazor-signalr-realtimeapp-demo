using System.Text.Json.Serialization;

namespace Shared.ChatServer.ApiDtos;

public record RemoveRoomMemberResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }

    [JsonPropertyName("roomId")]
    public long RoomId { get; init; }

    [JsonPropertyName("userId")]
    public required string UserId { get; init; }
}
