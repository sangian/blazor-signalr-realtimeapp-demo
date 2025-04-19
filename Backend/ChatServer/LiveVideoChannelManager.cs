using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Backend.ChatServer;

public sealed class LiveVideoChannelManager
{
    private static readonly ConcurrentDictionary<long, ConcurrentDictionary<(string, string), ChannelReader<(string, string)>>> RoomChannels = new();

    public bool AddRoomChannel(long roomId, string userId, string connectionId, ChannelReader<(string, string)> channel)
    {
        if (!RoomChannels.ContainsKey(roomId))
        {
            RoomChannels.TryAdd(roomId, new ConcurrentDictionary<(string, string), ChannelReader<(string, string)>>());
        }

        if (RoomChannels[roomId].ContainsKey((userId, connectionId)))
        {
            RoomChannels[roomId].TryRemove((userId, connectionId), out _);
        }

        return RoomChannels[roomId].TryAdd((userId, connectionId), channel);
    }

    public bool RemoveRoomChannel(long roomId, string userId, string connectionId)
    {
        if (RoomChannels.ContainsKey(roomId))
        {
            return RoomChannels[roomId].TryRemove((userId, connectionId), out _);
        }

        return false;
    }

    public void RemoveChannels(string userId, string connectionId)
    {
        RoomChannels.Where(kvp => kvp.Value.ContainsKey((userId, connectionId))).ToList().ForEach(kvp => {
            RoomChannels.TryRemove(kvp.Key, out _);
        });
    }

    public IEnumerable<ChannelReader<(string, string)>>? GetRoomChannels(long roomId)
    {
        return RoomChannels[roomId]
            .Values
            .AsEnumerable();
    }
}
