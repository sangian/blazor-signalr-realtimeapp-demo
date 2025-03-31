namespace Shared.ChatServer.HubEvents;
public record ChatRoomCreatedEvent
{
    public long Id { get; init; }
    public required string Name { get; init; }
    public SortedSet<string>? Members { get; init; }
}
