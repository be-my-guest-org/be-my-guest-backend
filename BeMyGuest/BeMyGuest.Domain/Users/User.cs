namespace BeMyGuest.Domain.Users;

public class User
{
    public string Username { get; } = default!;

    public string FirstName { get; } = default!;

    public string LastName { get; } = default!;

    public string Email { get; } = default!;

    public string Sub { get; } = default!;

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; }
}