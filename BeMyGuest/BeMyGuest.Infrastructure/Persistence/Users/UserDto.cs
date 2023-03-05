using System.Text.Json.Serialization;

namespace BeMyGuest.Infrastructure.Persistence.Users;

public record UserDto(
    [property: JsonPropertyName("pk")] string Pk,
    [property: JsonPropertyName("sk")] string Sk,
    [property: JsonPropertyName("firstName")]
    string FirstName,
    [property: JsonPropertyName("lastName")]
    string LastName,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("createdAt")]
    DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")]
    DateTime? UpdatedAt = null
)
{
    [JsonIgnore]
    public string Username => Pk[FieldSpecifierRange(Pk)];

    [JsonIgnore]
    public string Id => Sk[FieldSpecifierRange(Sk)];

    private static Range FieldSpecifierRange(string field)
    {
        return field.IndexOf("#", StringComparison.InvariantCultureIgnoreCase)..;
    }
}