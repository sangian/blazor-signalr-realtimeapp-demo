using System.Text.Json.Serialization;

namespace Shared;

public record TokenResponse
{
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; init; }
}
