namespace Shared.ChatServer.HubEvents;

public record ChatMessageReadEvent
{
    public long RoomId { get; init; }
    public required Guid[] MessageIds { get; init; }
    public DateTime ReadAt { get; init; }
    public string? Recipient { get; init; }
}
