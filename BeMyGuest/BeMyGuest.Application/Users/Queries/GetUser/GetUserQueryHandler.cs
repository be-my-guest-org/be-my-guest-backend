using BeMyGuest.Common.User;
using BeMyGuest.Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf.Types;

namespace BeMyGuest.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResult>
{
    private readonly CurrentUserData _currentUserData;
    private readonly ILogger<GetUserQueryHandler> _logger;
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(ILogger<GetUserQueryHandler> logger, IUserRepository userRepository, CurrentUserData currentUserData)
    {
        _logger = logger;
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