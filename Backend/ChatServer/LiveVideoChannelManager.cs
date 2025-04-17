using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Backend.ChatServer;

public sealed class LiveVideoChannelManager
{
    private static readonly ConcurrentDictionary<long, ConcurrentDictionary<(string, string), ChannelReader<string>>> Channels = new();

    public bool AddRoomChannel(long roomId, string userId, string connectionId, ChannelReader<string> channel)
    {
        if (!Channels.ContainsKey(roomId))
        {
            Channels.TryAdd(roomId, new ConcurrentDictionary<(string, string), ChannelReader<string>>());
        }
        return Channels[roomId].TryAdd((userId, connectionId), channel);
    }

    public bool RemoveRoomChannel(long roomId, string userId, string connectionId)
    {
        if (Channels.ContainsKey(roomId))
        {
            return Channels[roomId].TryRemove((userId, connectionId), out _);
        }

        return false;
    }

    public ConcurrentDictionary<(string, string), ChannelReader<string>>? GetRoomChannels(long roomId)
    {
        return Channels[roomId];
    }
}
