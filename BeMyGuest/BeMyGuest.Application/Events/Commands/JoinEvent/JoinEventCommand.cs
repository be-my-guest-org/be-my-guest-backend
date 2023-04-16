using MediatR;

namespace BeMyGuest.Application.Events.Commands.JoinEvent;

public record JoinEventCommand(
    Guid EventId
) : IRequest<JoinEventResult>;