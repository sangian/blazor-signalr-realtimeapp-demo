using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.ChatServer.ApiDtos;

public record SendMessageRequest
{
    [JsonPropertyName("messageId")]
    [Required(ErrorMessage = "MessageId is required")]
    public Guid MessageId { get; init; }

    [JsonPropertyName("message")]
    [Required(ErrorMessage = "Message is required")]
    public string? Message { get; init; }

    [JsonPropertyName("createdAt")]
    [Required(ErrorMessage = "CreatedAt is required")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("roomId")]
    [Required(ErrorMessage = "RoomId is required")]
    public long RoomId { get; init; }
}
