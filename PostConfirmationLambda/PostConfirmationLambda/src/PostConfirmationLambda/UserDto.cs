using System.Text.Json.Serialization;

namespace PostConfirmationLambda;

public record UserDto(
    string Username,
    string Email,
    [property: JsonPropertyName("createdAt")]
    DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")]
    DateTime? UpdatedAt = null
)
{
    [JsonPropertyName("pk")]
    public string Pk => $"USER#{Username}";

    [JsonPropertyName("sk")]
    public string Sk => $"PROFILE#{Email}";
}