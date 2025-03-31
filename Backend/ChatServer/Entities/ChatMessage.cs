namespace Backend.ChatServer.Entities;

public sealed class ChatMessage
{
    public Guid Id { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public long RoomId { get; set; }
    public string? Sender { get; set; }
}
