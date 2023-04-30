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
    // We're taking the numeric part of Username (e.g. Google_123456) and stripping non numeric characters. We're then
    // building a long based on that avoiding overflow
    public long Pk => long.Parse(Sk.Where(char.IsDigit).Take(19).ToArray());

    [JsonPropertyName("gsi1pk")]
    public string Gsi1Pk => $"USER#{Sub}";

    [JsonPropertyName("sk")]
    public string Sk => $"PROFILE#{Username}";
}