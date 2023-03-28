using MediatR;

namespace BeMyGuest.Application.Events.Queries.GetEvent;

public record GetEventQuery(Guid EventId) : IRequest<GetEventResult>;