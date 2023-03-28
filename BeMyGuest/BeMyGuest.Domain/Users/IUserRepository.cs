using BeMyGuest.Common.User;

namespace BeMyGuest.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetUser(CurrentUserData currentUserData);
}