using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Threading.Channels;

namespace Backend.ChatServer;

[Authorize]
public sealed class ChatServerHub(
    ILogger<ChatServerHub> logger,
    ChatServerUserManager chatServerUserManager,
    ChatRoomManager chatRoomManager,
    LiveVideoChannelManager liveVideoChannelManager) : Hub
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

    #region Live video stream
    public void RegisterClientChannel(long roomId, ChannelReader<string> streamChannel)
    {
        var userId = Context.User?.Identity?.Name;
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("ChatServerHub.RegisterClientChannel => Client has no user ID: {connectionId}", connectionId);
            return;
        }
        
        if (!chatRoomManager.IsMemberInChatRoom(roomId, userId!))
        {
            logger.LogWarning("ChatServerHub.RegisterClientChannel => User {userId} is not a member of chat room {roomId}", userId, roomId);
            return;
        }
            
        liveVideoChannelManager.AddRoomChannel(roomId, userId, connectionId, streamChannel);
    }

    public async Task StreamStarted(long roomId)
    {
        var userId = Context.User?.Identity?.Name;
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("ChatServerHub.StreamStarted => Client has no user ID: {connectionId}", connectionId);
            return;
        }
        
        if (!chatRoomManager.IsMemberInChatRoom(roomId, userId!))
        {
            logger.LogWarning("ChatServerHub.StreamStarted => User {userId} is not a member of chat room {roomId}", userId, roomId);
            return;
        }

        await Clients
            .Group(roomId.ToString())
            .SendAsync("StreamStarted", userId, roomId);
    }

    public async Task StreamStopped(long roomId)
    {
        var userId = Context.User?.Identity?.Name;
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("ChatServerHub.StreamStarted => Client has no user ID: {connectionId}", connectionId);
            return;
        }
        
        if (!chatRoomManager.IsMemberInChatRoom(roomId, userId!))
        {
            logger.LogWarning("ChatServerHub.StreamStarted => User {userId} is not a member of chat room {roomId}", userId, roomId);
            return;
        }

        await Clients
            .Group(roomId.ToString())
            .SendAsync("StreamStopped", userId, roomId);
    }

    public IEnumerable<ChannelReader<string>>? GetRoomChannels(long roomId)
    {
        var userId = Context.User?.Identity?.Name;
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("ChatServerHub.GetRoomChannels => Client has no user ID: {connectionId}", connectionId);
            return null;
        }
        
        if (!chatRoomManager.IsMemberInChatRoom(roomId, userId!))
        {
            logger.LogWarning("ChatServerHub.GetRoomChannels => User {userId} is not a member of chat room {roomId}", userId, roomId);
            return null;
        }
        
        return liveVideoChannelManager.GetRoomChannels(roomId)?
            .Values
            .AsEnumerable();
    }
    #endregion
}
