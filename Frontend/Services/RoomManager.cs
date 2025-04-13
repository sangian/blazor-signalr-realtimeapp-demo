using Frontend.Models;
using System.Collections.Concurrent;

namespace Frontend.Services;

public sealed class RoomManager : IDisposable
{
    private readonly ConcurrentDictionary<long, RoomModel> Rooms = new();
    private static RoomModel? SelectedRoom = null;

    public event Action? RoomsChanged;
    public event Action? RoomSelected;
    public event Action<long, MessageModel>? MessageReceived; // New message received
    public event Action<long, MessageModel>? MessageSent; // Message sent
    public event Action<long, MessageDeliveryReceiptModel>? MessageDelivered; // Message delivery receipt received
    public event Action<long>? MessagesRead; // Message read receipts received

    #region Rooms management

    public ICollection<RoomModel> GetRooms()
    {
        return Rooms.Values;
    }

    public bool IsRoomExists(long roomId)
    {
        return Rooms.ContainsKey(roomId);
    }

    public RoomModel? GetRoom(long roomId)
    {
        return Rooms.TryGetValue(roomId, out var room) ? room : null;
    }

    public bool AddRooms(IEnumerable<RoomModel> rooms)
    {
        ArgumentNullException.ThrowIfNull(rooms);
        try
        {
            Parallel.ForEach(rooms, room =>
            {
                Rooms.AddOrUpdate(room.Id, room, (_, _) => room);
            });
            RoomsChanged?.Invoke();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool AddRoom(RoomModel room)
    {
        ArgumentNullException.ThrowIfNull(room);
        var result = Rooms.TryAdd(room.Id, room);
        if (result) RoomsChanged?.Invoke();
        return result;
    }

    public bool AddOrUpdateRoom(RoomModel room)
    {
        ArgumentNullException.ThrowIfNull(room);
        var result = Rooms.AddOrUpdate(room.Id, room, (_, _) => room) != null;
        if (result) RoomsChanged?.Invoke();
        return result;
    }

    public bool RemoveRoom(long roomId)
    {
        var result = Rooms.TryRemove(roomId, out _);
        if (result) RoomsChanged?.Invoke();
        return result;
    }

    public bool SelectRoom(long roomId)
    {
        if (roomId == SelectedRoom?.Id) return true;

        if (Rooms.TryGetValue(roomId, out var room))
        {
            SelectedRoom = room;
            RoomSelected?.Invoke();
            return true;
        }
        return false;
    }

    public void SetRoomHasUnreadMessages(long roomId, bool hasUnreadMessages)
    {
        if (Rooms.TryGetValue(roomId, out var room))
        {
            room.HasUnreadMessages = hasUnreadMessages;
            RoomsChanged?.Invoke();
        }
    }

    public RoomModel? GetSelectedRoom()
    {
        return SelectedRoom;
    }

    #endregion


    #region Messages management

    public void InitRoomMessages(long roomId, IEnumerable<MessageModel> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        if (Rooms.TryGetValue(roomId, out var room))
        {
            room.Messages = new SortedList<Guid, MessageModel>(messages.ToDictionary(m => m.Id));
        }
    }

    public bool AddOrUpdateRoomMessage(long roomId, MessageModel message, bool isSent = false)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (Rooms.TryGetValue(roomId, out var room))
        {
            if (room.Messages.TryGetValue(message.Id, out var existingMessage))
            {
                // Update the existing message
                existingMessage.Message = message.Message;
                existingMessage.CreatedAt = message.CreatedAt;
                existingMessage.SentAt = message.SentAt;
                existingMessage.Sender = message.Sender;
                existingMessage.DeliveryReceipts = message.DeliveryReceipts;
                existingMessage.ReadReceipts = message.ReadReceipts;
            }
            else
            {
                // Add the new message
                room.Messages.Add(message.Id, message);
            }

            if (!isSent)
            {
                MessageReceived?.Invoke(roomId, message);
            }
            else
            {
                MessageSent?.Invoke(roomId, message);
            }

            return true;
        }
        return false;
    }

    public void AddDeliveryReceipt(long roomId, MessageDeliveryReceiptModel receipt)
    {
        ArgumentNullException.ThrowIfNull(receipt);
        if (Rooms.TryGetValue(roomId, out var room))
        {
            if (room.Messages.TryGetValue(receipt.MessageId, out var message))
            {
                message.DeliveryReceipts.Add(receipt);
                MessageDelivered?.Invoke(roomId, receipt);
            }
        }
    }

    public void AddReadReceipts(long roomId, IEnumerable<MessageReadReceiptModel> receipts)
    {
        ArgumentNullException.ThrowIfNull(receipts);
        if (Rooms.TryGetValue(roomId, out var room))
        {
            var groupedReceipts = receipts.GroupBy(r => r.MessageId);

            foreach (var group in groupedReceipts)
            {
                if (room.Messages.TryGetValue(group.Key, out var message))
                {
                    message.ReadReceipts.AddRange(group);
                }
            }

            MessagesRead?.Invoke(roomId);
        }
    }

    #endregion

    public void Dispose()
    {
        RoomsChanged = null;
        RoomSelected = null;
        MessageReceived = null;
        MessageDelivered = null;
        MessagesRead = null;
    }
}
