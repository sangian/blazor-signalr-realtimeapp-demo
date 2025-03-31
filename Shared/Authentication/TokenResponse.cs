using System.Text.Json.Serialization;

namespace Shared.Authentication;

public record TokenResponse
{
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; init; }
}
