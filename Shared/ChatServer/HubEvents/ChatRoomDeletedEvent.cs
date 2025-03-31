namespace Shared.ChatServer.HubEvents;

public record ChatRoomDeletedEvent
{
    public long Id { get; init; }
    public required string Name { get; init; }
    public SortedSet<string>? Members { get; init; }
}
