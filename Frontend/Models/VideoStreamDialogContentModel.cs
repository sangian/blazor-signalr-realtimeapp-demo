using Microsoft.AspNetCore.SignalR.Client;

namespace Frontend.Models;

public sealed class VideoStreamDialogContentModel
{
    public string? UserId { get; set; }
    public long? RoomId { get; set; }
    public HubConnection? HubConnection { get; set; }
}
