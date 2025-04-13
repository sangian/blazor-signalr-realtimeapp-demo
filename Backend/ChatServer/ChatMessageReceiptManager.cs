using Backend.ChatServer.Entities;
using System.Collections.Concurrent;

namespace Backend.ChatServer;

public sealed class ChatMessageReceiptManager
{
    private static readonly ConcurrentDictionary<long, ConcurrentBag<ChatMessageDeliveryReceipt>> DeliveryReceipts = new();
    private static readonly ConcurrentDictionary<long, ConcurrentBag<ChatMessageReadReceipt>> ReadReceipts = new();

    public bool AddDeliveryReceipt(ChatMessageDeliveryReceipt receipt)
    {
        if (!DeliveryReceipts.TryGetValue(receipt.RoomId, out var value))
        {
            value = [];
            DeliveryReceipts[receipt.RoomId] = value;
        }

        value.Add(receipt);
        return true;
    }

    public bool AddReadReceipt(ChatMessageReadReceipt receipt)
    {
        if (!ReadReceipts.TryGetValue(receipt.RoomId, out var value))
        {
            value = [];
            ReadReceipts[receipt.RoomId] = value;
        }

        value.Add(receipt);
        return true;
    }

    public void AddReadReceipts(ChatMessageReadReceipt[] receipts)
    {
        foreach (var receipt in receipts)
        {
            AddReadReceipt(receipt);
        }
    }

    public IReadOnlyCollection<ChatMessageDeliveryReceipt> GetDeliveryReceipts(long roomId)
    {
        if (DeliveryReceipts.TryGetValue(roomId, out var receipts))
        {
            return receipts.ToList().AsReadOnly();
        }
        return [];
    }

    public IReadOnlyCollection<ChatMessageReadReceipt> GetReadReceipts(long roomId)
    {
        if (ReadReceipts.TryGetValue(roomId, out var receipts))
        {
            return receipts.ToList().AsReadOnly();
        }
        return [];
    }
}
