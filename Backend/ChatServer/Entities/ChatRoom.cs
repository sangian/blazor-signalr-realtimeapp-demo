namespace Backend.ChatServer.Entities;

public sealed class ChatRoom
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public SortedSet<string> Members { get; set; } = [];
}
