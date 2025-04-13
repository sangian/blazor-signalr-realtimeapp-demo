using Backend.ChatServer.Entities;
using System.Collections.Concurrent;

namespace Backend.ChatServer;

public sealed class ChatMessageManager
{
    private static readonly ConcurrentDictionary<long, SortedList<Guid, ChatMessage>> ChatMessages = new();

    public bool AddMessage(ChatMessage message)
    {
        if (!ChatMessages.TryGetValue(message.RoomId, out var value))
        {
            value = [];
            ChatMessages[message.RoomId] = value;
        }

        if (!value.ContainsKey(message.Id))
        {
            value.Add(message.Id, message);
            return true;
        }

        return false;
    }

    public bool IsMessageExists(long roomId, Guid id)
    {
        return ChatMessages.TryGetValue(roomId, out var room) &&
            room.ContainsKey(id);
    }

    public IReadOnlyCollection<ChatMessage> GetMessagesByRoomId(long roomId)
    {
        if (ChatMessages.TryGetValue(roomId, out var value))
        {
            return [.. value.Values];
        }
        return [];
    }

    public IReadOnlyCollection<Guid> GetMessageIdsByRoomId(long roomId)
    {
        if (ChatMessages.TryGetValue(roomId, out var value))
        {
            return [.. value.Keys];
        }
        return [];
    }
}
