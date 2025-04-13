namespace Frontend.Models;

public sealed class MessageModel
{
    public Guid Id { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Sender { get; set; }
    public List<MessageDeliveryReceiptModel> DeliveryReceipts { get; set; } = [];
    public List<MessageReadReceiptModel> ReadReceipts { get; set; } = [];

    public bool IsSelfMessage(string userId) => Sender!.Equals(userId);
    public bool IsSent => SentAt.HasValue;
    public bool IsDelivered(int roomMembers) => DeliveryReceipts.Count >= (roomMembers - 1);
    public bool IsRead(int roomMembers) => ReadReceipts.Count >= (roomMembers -1);
}

public sealed class MessageDeliveryReceiptModel
{
    public Guid MessageId { get; set; }
    public DateTime DeliveredAt { get; set; }
    public string? Recipient { get; set; }
}

public sealed class MessageReadReceiptModel
{
    public Guid MessageId { get; set; }
    public DateTime ReadAt { get; set; }
    public string? Recipient { get; set; }
}
