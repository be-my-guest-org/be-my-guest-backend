using MediatR;
using Microsoft.Extensions.Logging;

namespace BeMyGuest.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResult>
{
    private readonly ILogger<GetUserQueryHandler> _logger;

    public GetUserQueryHandler(ILogger<GetUserQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<GetUserResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received get user query for user {Email}", request.Email);

        return await Task.FromResult(
            new GetUserResult("test", "test", "test", "test", DateTime.UtcNow, DateTime.UtcNow));
    }
}