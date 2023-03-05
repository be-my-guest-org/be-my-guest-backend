namespace BeMyGuest.Contracts.Users;

public record GetUserResponse(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime UpdatedAt
);