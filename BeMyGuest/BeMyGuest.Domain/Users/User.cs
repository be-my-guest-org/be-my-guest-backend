namespace BeMyGuest.Domain.Users;

public class User
{
    public string Username { get; init; } = default!;

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public string Email { get; init; } = default!;

    public string Id { get; init; } = default!;

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}