namespace BeMyGuest.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetUser();
}