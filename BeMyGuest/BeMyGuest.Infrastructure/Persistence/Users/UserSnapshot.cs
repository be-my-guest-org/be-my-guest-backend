using System.Text.Json.Serialization;

namespace BeMyGuest.Infrastructure.Persistence.Users;

public record UserSnapshot(
    [property: JsonPropertyName("pk")]
    string Pk,
    [property: JsonPropertyName("sk")]
    string Sk,
    [property: JsonPropertyName("firstName")]
    string FirstName,
    [property: JsonPropertyName("lastName")]
    string LastName,
    [property: JsonPropertyName("email")]
    string Email,
    [property: JsonPropertyName("createdAt")]
    DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")]
    DateTime? UpdatedAt = null
);