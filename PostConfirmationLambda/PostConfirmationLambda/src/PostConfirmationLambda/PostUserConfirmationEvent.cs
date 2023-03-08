using System.Text.Json.Serialization;

namespace PostConfirmationLambda;

public record PostUserConfirmationEvent(
    [property: JsonPropertyName("userName")]
    string Username,
    [property: JsonPropertyName("request")]
    Request Request)
{
    public UserDto ToUserDto()
    {
        return new UserDto(
            Username,
            Request.UserAttributes.FirstName,
            Request.UserAttributes.LastName,
            Request.UserAttributes.Email,
            Request.UserAttributes.Sub,
            DateTime.UtcNow);
    }
}

public record Request(
    [property: JsonPropertyName("userAttributes")]
    UserAttributes UserAttributes);

public record UserAttributes(
    [property: JsonPropertyName("email")]
    string Email,
    [property: JsonPropertyName("given_name")]
    string FirstName,
    [property: JsonPropertyName("family_name")]
    string LastName,
    [property: JsonPropertyName("sub")]
    string Sub
);