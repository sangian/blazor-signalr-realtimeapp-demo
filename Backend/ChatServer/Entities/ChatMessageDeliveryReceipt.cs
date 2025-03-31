namespace Backend.ChatServer.Entities;

public sealed class ChatMessageDeliveryReceipt
{
    public long RoomId { get; set; }
    public Guid MessageId { get; set; }
    public DateTime DeliveredAt { get; set; }
    public string? Recipient { get; set; }
}
