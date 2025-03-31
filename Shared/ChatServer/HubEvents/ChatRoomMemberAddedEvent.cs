namespace Shared.ChatServer.HubEvents;

public record ChatRoomMemberAddedEvent
{
    public long RoomId { get; init; }
    public required string UserId { get; init; }
}
