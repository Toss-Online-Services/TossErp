using System.Text.Json.Serialization;

namespace Toss.Application.Common.Models.Auth;

public record AuthTokenResult(
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("refreshToken")] string RefreshToken,
    [property: JsonPropertyName("expiresIn")] long ExpiresIn,
    [property: JsonPropertyName("user")] IReadOnlyDictionary<string, object?> User);

