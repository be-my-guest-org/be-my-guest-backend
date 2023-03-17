using MediatR;

namespace BeMyGuest.Application.Users.Queries.GetUser;

public record GetUserQuery : IRequest<GetUserResult>;