namespace BeMyGuest.Contracts.Users;

public record UserResponse(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime UpdatedAt
);