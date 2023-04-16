using BeMyGuest.Common.User;
using BeMyGuest.Domain.Users;
using MediatR;
using OneOf.Types;

namespace BeMyGuest.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResult>
{
    private readonly CurrentUserData _currentUserData;
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository, CurrentUserData currentUserData)
    {
        _userRepository = userRepository;
        _currentUserData = currentUserData;
    }

    public async Task<GetUserResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUser(_currentUserData);

        if (user is null)
        {
            return new NotFound();
        }

        return user;
    }
}