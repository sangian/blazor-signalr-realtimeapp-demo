using Frontend.Models;
using System.Collections;
using System.Collections.Concurrent;

namespace Frontend.Services;

public sealed class RoomManager
{
    private static readonly ConcurrentDictionary<long, RoomModel> Rooms = new();

    public static event Action? RoomsChanged;

    public static ICollection<RoomModel> GetRooms()
    {
        return Rooms.Values;
    }

    public static RoomModel? GetRoom(long roomId)
    {
        return Rooms.TryGetValue(roomId, out var room) ? room : null;
    }

    public static bool AddRooms(IEnumerable<RoomModel> rooms)
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

    public static bool AddRoom(RoomModel room)
    {
        ArgumentNullException.ThrowIfNull(room);
        var result = Rooms.TryAdd(room.Id, room);
        if (result) RoomsChanged?.Invoke();
        return result;
    }

    public static bool AddOrUpdateRoom(RoomModel room)
    {
        ArgumentNullException.ThrowIfNull(room);
        var result = Rooms.AddOrUpdate(room.Id, room, (_, _) => room) != null;
        if (result) RoomsChanged?.Invoke();
        return result;
    }

    public static bool RemoveRoom(long roomId)
    {
        var result = Rooms.TryRemove(roomId, out _);
        if (result) RoomsChanged?.Invoke();
        return result;
    }
}
