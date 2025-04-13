using System.Text.Json.Serialization;

namespace Shared.ChatServer.HubEvents;

public record ChatMessageSentEvent
{
    [JsonPropertyName("roomId")]
    public long RoomId { get; init; }

    [JsonPropertyName("id")]
    public Guid MessageId { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("sentAt")]
    public DateTime? SentAt { get; init; }

    [JsonPropertyName("sender")]
    public string? Sender { get; init; }
}
