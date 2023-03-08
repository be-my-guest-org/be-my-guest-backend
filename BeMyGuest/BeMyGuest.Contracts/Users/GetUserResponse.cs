namespace BeMyGuest.Contracts.Users;

public record GetUserResponse(
    string UserId,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);