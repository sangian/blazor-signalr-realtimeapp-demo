namespace Shared.ChatServer.HubEvents;

public record ChatMessageDeliveredEvent
{
    public long RoomId { get; init; }
    public Guid MessageId { get; init; }
    public DateTime DeliveredAt { get; init; }
    public string? Recipient { get; init; }
}
