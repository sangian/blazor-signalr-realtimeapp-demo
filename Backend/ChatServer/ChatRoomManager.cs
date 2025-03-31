using Backend.ChatServer.Entities;
using System.Collections.Concurrent;

namespace Backend.ChatServer;

public sealed class ChatRoomManager
{
    private static readonly ConcurrentDictionary<long, ChatRoom> ChatRooms = new();

    public IReadOnlyCollection<ChatRoom> GetChatRooms()
    {
        return ChatRooms.Values.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<ChatRoom> GetMemberChatRooms(string member)
    {
        return ChatRooms.Values
            .Where(room => room.Members.Contains(member))
            .ToList()
            .AsReadOnly();
    }

    public bool IsMemberInChatRoom(long roomId, string member)
    {
        return ChatRooms.TryGetValue(roomId, out var chatRoom) && chatRoom.Members.Contains(member);
    }

    public bool IsChatRoomExists(long roomId)
    {
        return ChatRooms.ContainsKey(roomId);
    }

    public long GetNextChatRoomId()
    {
        return ChatRooms.Keys.Count == 0 ? 1 : ChatRooms.Keys.Max() + 1;
    }

    public ChatRoom? GetChatRoom(long roomId)
    {
        return ChatRooms.TryGetValue(roomId, out ChatRoom? value) ? value : null;
    }

    public bool CreateChatRoom(ChatRoom newChatRoom)
    {
        return ChatRooms.TryAdd(newChatRoom.Id, newChatRoom);
    }

    public bool UpdateChatRoom(ChatRoom updatedChatRoom)
    {
        if (!IsChatRoomExists(updatedChatRoom.Id))
        {
            return false;
        }

        var oldChatRoom = GetChatRoom(updatedChatRoom.Id);
        return ChatRooms.TryUpdate(updatedChatRoom.Id, updatedChatRoom, oldChatRoom!);
    }

    public bool RemoveChatRoom(long roomId)
    {
        return ChatRooms.TryRemove(roomId, out _);
    }

    public bool AddMemberToChatRoom(long roomId, string member)
    {
        if (!IsChatRoomExists(roomId))
        {
            return false;
        }
        var chatRoom = GetChatRoom(roomId);
        chatRoom!.Members.Add(member);
        return UpdateChatRoom(chatRoom);
    }

    public bool RemoveMemberFromChatRoom(long roomId, string member)
    {
        if (!IsChatRoomExists(roomId))
        {
            return false;
        }
        var chatRoom = GetChatRoom(roomId);
        chatRoom!.Members.Remove(member);
        return UpdateChatRoom(chatRoom);
    }
}
