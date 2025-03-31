using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.ChatServer.ApiDtos;

public record CreateRoomRequest
{
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Room name is required")]
    public string? Name { get; init; }

    [JsonPropertyName("members")]
    [Required(ErrorMessage = "Room members is required")]
    [MinLength(2, ErrorMessage = "Room must have at least 2 members")]
    public SortedSet<string>? Members { get; init; }
}
