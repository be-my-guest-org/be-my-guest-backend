using MediatR;

namespace BeMyGuest.Application.Events.Commands.JoinEvent;

public record JoinEventCommand(
    Guid HostId,
    Guid EventId
) : IRequest<JoinEventResult>;