using Microsoft.AspNetCore.SignalR;
using Shared;
using Shared.ChatServer.HubEvents;

namespace Backend.ChatServer;

public sealed class ChatServerService(
    IHubContext<ChatServerHub> chatServerHubContext,
    ChatServerUserManager chatServerUserManager)
{
    public async Task NotifyChatRoomCreated(ChatRoomCreatedEvent room)
    {
        // Add all members to the room group
        foreach (var userId in room.Members!)
        {
            var userConnections = chatServerUserManager.GetUserConnections(userId);

            foreach (var connectionId in userConnections)
            {
                await chatServerHubContext.Groups.AddToGroupAsync(connectionId, room.Id.ToString(), default);
            }
        }

        await chatServerHubContext.Clients
            .Group(room.Id.ToString())
            .SendAsync(Constants.CLIENT_CHAT_ROOM_CREATED, room, default);
    }

    public async Task NotifyChatRoomMemberAdded(long roomId, string userId)
    {
        var userConnections = chatServerUserManager.GetUserConnections(userId);
        foreach (var connectionId in userConnections)
        {
            await chatServerHubContext.Groups.AddToGroupAsync(connectionId, roomId.ToString(), default);
        }

        await chatServerHubContext.Clients
            .Group(roomId.ToString())
            .SendAsync(Constants.CLIENT_CHAT_ROOM_MEMBER_ADDED, new { roomId, userId }, default);
    }

    public async Task NotifyChatRoomMemberRemoved(long roomId, string userId)
    {
        await chatServerHubContext.Clients
            .Group(roomId.ToString())
            .SendAsync(Constants.CLIENT_CHAT_ROOM_MEMBER_REMOVED, new { roomId, userId }, default);

        var userConnections = chatServerUserManager.GetUserConnections(userId);
        foreach (var connectionId in userConnections)
        {
            await chatServerHubContext.Groups.RemoveFromGroupAsync(connectionId, roomId.ToString(), default);
        }
    }

    public async Task NotifyChatRoomDeleted(ChatRoomDeletedEvent room)
    {
        await chatServerHubContext.Clients
            .Group(room.Id.ToString())
            .SendAsync(Constants.CLIENT_CHAT_ROOM_DELETED, room, default);

        // Remove all members from the room group
        foreach (var userId in room.Members!)
        {
            var userConnections = chatServerUserManager.GetUserConnections(userId);

            foreach (var connectionId in userConnections)
            {
                await chatServerHubContext.Groups.RemoveFromGroupAsync(connectionId, room.Id.ToString(), default);
            }
        }
    }

    public async Task NotifyChatMessageSent(ChatMessageSentEvent chatMessage)
    {
        await chatServerHubContext.Clients
            .Group(chatMessage.RoomId.ToString())
            .SendAsync(Constants.CLIENT_CHAT_MESSAGE_SENT, chatMessage, default);
    }

    public async Task NotifyChatMessageDelivered(ChatMessageDeliveredEvent receipt)
    {
        await chatServerHubContext.Clients
            .Group(receipt.RoomId.ToString())
            .SendAsync(Constants.CLIENT_CHAT_MESSAGE_DELIVERED, receipt, default);
    }

    public async Task NotifyChatMessageRead(ChatMessageReadEvent receipt)
    {
        await chatServerHubContext.Clients
            .Group(receipt.RoomId.ToString())
            .SendAsync(Constants.CLIENT_CHAT_MESSAGE_READ, receipt, default);
    }
}
