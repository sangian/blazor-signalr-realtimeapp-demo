namespace Backend.ChatServer.Entities;

public sealed class ChatMessageReadReceipt
{
    public long RoomId { get; set; }
    public Guid MessageId { get; set; }
    public DateTime ReadAt { get; set; }
    public string? Recipient { get; set; }
}
