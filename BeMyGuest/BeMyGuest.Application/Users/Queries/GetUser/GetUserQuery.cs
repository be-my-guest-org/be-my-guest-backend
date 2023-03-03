using MediatR;

namespace BeMyGuest.Application.Users.Queries.GetUser;

public record GetUserQuery(string UserId, string Email) : IRequest<GetUserResult>;