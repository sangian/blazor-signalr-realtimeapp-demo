using System.Text.Json.Serialization;

namespace Shared.ChatServer.ApiDtos;

public record GetMessagesResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }

    [JsonPropertyName("roomId")]
    public long RoomId { get; init; }

    [JsonPropertyName("messages")]
    public GetMessagesItem[]? Messages { get; init; }
}

public record GetMessagesItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("sentAt")]
    public DateTime? SentAt { get; init; }

    [JsonPropertyName("sender")]
    public string? Sender { get; init; }

    [JsonPropertyName("deliveryReceipts")]
    public GetMessagesDeliveryReceiptItem[]? DeliveryReceipts { get; init; }

    [JsonPropertyName("readReceipts")]
    public GetMessagesReadReceiptItem[]? ReadReceipts { get; init; }
}

public record GetMessagesDeliveryReceiptItem
{
    [JsonPropertyName("messageId")]
    public Guid MessageId { get; init; }

    [JsonPropertyName("recipient")]
    public string? Recipient { get; init; }

    [JsonPropertyName("deliveredAt")]
    public DateTime DeliveredAt { get; init; }
}

public record GetMessagesReadReceiptItem
{
    [JsonPropertyName("messageId")]
    public Guid MessageId { get; init; }

    [JsonPropertyName("recipient")]
    public string? Recipient { get; init; }

    [JsonPropertyName("readAt")]
    public DateTime ReadAt { get; init; }
}
