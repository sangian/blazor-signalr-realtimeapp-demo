using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared;

namespace Backend.ChatServer;

[Authorize]
public sealed class ChatServerHub(
    ILogger<ChatServerHub> logger,
    ChatServerUserManager chatServerUserManager,
    ChatRoomManager chatRoomManager) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("ChatServerHub => Client connected with no user ID: {connectionId}", connectionId);
        }
        else
        {
            logger.LogInformation("ChatServerHub => Client connected: {userId} {connectionId}", userId, connectionId);
            chatServerUserManager.AddUser(userId, connectionId);

            var userRooms = chatRoomManager.GetMemberChatRooms(userId!);
            foreach (var room in userRooms)
            {
                await JoinChatRoom(room.Id, userId, connectionId);
            }
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Identity?.Name;
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("ChatServerHub => Client disconnected with no user ID: {connectionId}", connectionId);
        }
        else
        {
            logger.LogInformation("ChatServerHub => Client disconnected: {userId} {connectionId}", userId, connectionId);
            chatServerUserManager.RemoveUser(userId, connectionId);
        }

        if (exception is not null)
        {
            logger.LogError(exception, "ChatServerHub => Client {connectionId} disconnected with error: {Error}", connectionId, exception.Message);
        }

        return base.OnDisconnectedAsync(exception);
    }

    public void Pong()
    {
        var userId = Context.User?.Identity?.Name;
        logger.LogInformation("Received PONG from user {userId}", userId);
    }

    public void IsTyping(long roomId)
    {
        var userId = Context.User?.Identity?.Name;
        if (chatRoomManager.IsMemberInChatRoom(roomId, userId!))
        {
            Clients.Group(roomId.ToString()).SendAsync(Constants.CLIENT_CHAT_MESSAGE_TYPING, roomId, userId);
        }
    }

    private async Task JoinChatRoom(long roomId, string userId, string connectionId)
    {
        if (!chatRoomManager.IsChatRoomExists(roomId))
        {
            logger.LogWarning("ChatServerHub => User {userId} tried to join non-existing chat room {roomId}", userId, roomId);
            return;
        }
        if (!chatRoomManager.IsMemberInChatRoom(roomId, userId!))
        {
            logger.LogWarning("ChatServerHub => User {userId} tried to join chat room {roomId} without being a member", userId, roomId);
            return;
        }

        await Groups.AddToGroupAsync(connectionId, roomId.ToString());

        logger.LogInformation("ChatServerHub => User {userId} joined chat room {roomId}", userId, roomId);
    }
}
