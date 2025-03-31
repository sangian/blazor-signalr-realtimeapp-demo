using Backend.ChatServer.Entities;
using System.Collections.Concurrent;

namespace Backend.ChatServer;

public sealed class ChatMessageManager
{
    private static readonly ConcurrentDictionary<long, (ConcurrentBag<ChatMessage> Messages, HashSet<Guid> MessageIds)> ChatMessagesByRoomId = new();

    public bool AddMessage(ChatMessage message)
    {
        if (!ChatMessagesByRoomId.TryGetValue(message.RoomId, out var value))
        {
            value = (new ConcurrentBag<ChatMessage>(), new HashSet<Guid>());
            ChatMessagesByRoomId[message.RoomId] = value;
        }

        if (value.MessageIds.Add(message.Id))
        {
            value.Messages.Add(message);
            return true;
        }

        return false;
    }

    public bool IsMessageExists(long roomId, Guid id)
    {
        return ChatMessagesByRoomId.TryGetValue(roomId, out var room) &&
            room.MessageIds.Contains(id);
    }

    public IReadOnlyCollection<ChatMessage> GetMessagesByRoomId(long roomId)
    {
        if (ChatMessagesByRoomId.TryGetValue(roomId, out var value))
        {
            return [.. value.Messages.OrderBy(m => m.CreatedAt)];
        }
        return [];
    }

    public IReadOnlyCollection<Guid> GetMessageIdsByRoomId(long roomId)
    {
        if (ChatMessagesByRoomId.TryGetValue(roomId, out var value))
        {
            return [.. value.MessageIds];
        }
        return [];
    }
}
