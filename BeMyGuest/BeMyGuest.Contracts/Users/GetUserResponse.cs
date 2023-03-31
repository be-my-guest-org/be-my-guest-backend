namespace BeMyGuest.Contracts.Users;

public record GetUserResponse(
    Guid UserId,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);