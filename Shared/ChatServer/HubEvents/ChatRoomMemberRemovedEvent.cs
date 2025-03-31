namespace Shared.ChatServer.HubEvents;

public record ChatRoomMemberRemovedEvent
{
    public long RoomId { get; init; }
    public required string UserId { get; init; }
}
