namespace BeMyGuest.Common.User;

public class CurrentUserData
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = default!;
}