namespace BeMyGuest.Domain.Users;

public class User
{
    private User(
        string username,
        string firstName,
        string lastName,
        string email,
        string id,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public string Username { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string Email { get; }

    public string Id { get; }

    public DateTime CreatedAt { get; }

    public DateTime? UpdatedAt { get; }

    public static User Create(
        string username,
        string firstName,
        string lastName,
        string email,
        string id,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new User(
            username,
            firstName,
            lastName,
            email,
            id,
            createdAt,
            updatedAt);
    }
}