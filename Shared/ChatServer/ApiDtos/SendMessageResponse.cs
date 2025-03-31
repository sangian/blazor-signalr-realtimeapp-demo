using System.Text.Json.Serialization;

namespace Shared.ChatServer.ApiDtos;

public record SendMessageResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }

    [JsonPropertyName("messageId")]
    public Guid MessageId { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("sentAt")]
    public DateTime? SentAt { get; init; }

    [JsonPropertyName("roomId")]
    public long RoomId { get; init; }

    [JsonPropertyName("sender")]
    public string? Sender { get; init; }
}
