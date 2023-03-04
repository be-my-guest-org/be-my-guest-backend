using System.Text.Json.Serialization;

namespace PostConfirmationLambda;

public record UserDto(
    [property: JsonIgnore]
    string Username,
    [property: JsonPropertyName("firstName")]
    string FirstName,
    [property: JsonPropertyName("lastName")]
    string LastName,
    [property: JsonPropertyName("email")]
    string Email,
    [property: JsonIgnore]
    string Sub,
    [property: JsonPropertyName("createdAt")]
    DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")]
    DateTime? UpdatedAt = null
)
{
    [JsonPropertyName("pk")]
    public string Pk => $"USER#{Sub}";

    [JsonPropertyName("sk")]
    public string Sk => $"PROFILE#{Username}";
}