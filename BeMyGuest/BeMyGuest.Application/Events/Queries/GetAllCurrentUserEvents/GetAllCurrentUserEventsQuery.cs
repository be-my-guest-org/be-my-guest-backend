using MediatR;

namespace BeMyGuest.Application.Events.Queries.GetAllCurrentUserEvents;

public record GetAllCurrentUserEventsQuery : IRequest<GetAllCurrentUserEventsResult>;