using BeMyGuest.Domain.Common.Models;

namespace BeMyGuest.Domain.Users;

public class User : EntityBase<string>
{
    private User(
        string username,
        string firstName,
        string lastName,
        string email,
        string id,
        DateTime createdAt,
        DateTime? updatedAt)
        : base(id, createdAt, updatedAt)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public string Username { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string Email { get; }

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