using System.Numerics;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
    Guid Sub,
    [property: JsonPropertyName("createdAt")]
    DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")]
    DateTime? UpdatedAt = null
)
{
    [JsonPropertyName("pk")]
    public long Pk => new Random().NextInt64();

    [JsonPropertyName("gsi1pk")]
    public string Gsi1Pk => $"USER#{Sub}";

    [JsonPropertyName("sk")]
    public string Sk => $"PROFILE#{Username}";
}