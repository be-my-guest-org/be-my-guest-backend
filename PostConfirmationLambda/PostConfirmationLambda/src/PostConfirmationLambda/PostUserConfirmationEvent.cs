using System.Text.Json.Serialization;

namespace PostConfirmationLambda;

public record PostUserConfirmationEvent(
    [property: JsonPropertyName("username")]
    string Username,
    [property: JsonPropertyName("request")]
    Request Request)
{
    public UserDto ToUserDto()
    {
        return new UserDto(
            Username,
            Request.UserAttributes.Email,
            DateTime.UtcNow);
    }
}

public record Request(
    [property: JsonPropertyName("userAttributes")]
    UserAttributes UserAttributes);

public record UserAttributes(
    [property: JsonPropertyName("email")] string Email);