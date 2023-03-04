using BeMyGuest.Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

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

        return await Task.FromResult(
            new GetUserResult("test", "test", "test", "test", DateTime.UtcNow, DateTime.UtcNow));
    }
}