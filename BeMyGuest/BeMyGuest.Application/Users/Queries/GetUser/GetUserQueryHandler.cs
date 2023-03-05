using BeMyGuest.Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf.Types;

namespace BeMyGuest.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResult>
{
    private readonly ILogger<GetUserQueryHandler> _logger;
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(ILogger<GetUserQueryHandler> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<GetUserResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received get user query for user {Username}", request.Username);

        var user = await _userRepository.GetUser(request.UserId, request.Username);

        if (user is null)
        {
            return new NotFound();
        }

        return user;
    }
}