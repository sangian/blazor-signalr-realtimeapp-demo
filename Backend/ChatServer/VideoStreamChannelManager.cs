using Shared.TelemetryServer;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Backend.ChatServer;

public sealed class VideoStreamChannelManager
{
    private static readonly ConcurrentDictionary<string, Channel<string>> Channels = new();

    public bool AddChannel(string connectionId, Channel<string> channel)
    {
        return Channels.TryAdd(connectionId, channel);
    }

    public bool RemoveChannel(string connectionId)
    {
        var result = Channels.TryRemove(connectionId, out var channel);

        if (result)
        {
            channel?.Writer.Complete();
        }

        return result;
    }

    public bool TryGetChannel(string connectionId, out Channel<string>? channel)
    {
        return Channels.TryGetValue(connectionId, out channel);
    }

    public IReadOnlyCollection<Channel<string>> GetChannels()
    {
        return Channels.Values.ToList().AsReadOnly();
    }

    public void StreamVideo(string videoFrame)
    {
        Parallel.ForEach(Channels.Values, channel =>
        {
            channel.Writer.TryWrite(videoFrame);
        });
    }
}
