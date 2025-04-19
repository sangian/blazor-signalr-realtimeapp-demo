using System.Threading.Channels;

namespace Shared.ChatServer.Models;
public sealed class RemoteVideoStreams
{
    public IEnumerable<ChannelReader<(string, string)>>? Streams { get; set; } = [];
}
