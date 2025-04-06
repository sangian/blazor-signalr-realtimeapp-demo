namespace Frontend.Models;

public sealed class RoomModel
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public SortedSet<string> Members { get; set; } = [];
}
