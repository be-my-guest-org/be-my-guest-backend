namespace BeMyGuest.Application.Users.Queries.GetUser;

public record GetUserResult(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime UpdatedAt);