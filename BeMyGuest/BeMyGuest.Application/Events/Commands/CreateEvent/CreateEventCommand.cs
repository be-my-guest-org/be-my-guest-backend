using BeMyGuest.Common.Common;
using MediatR;

namespace BeMyGuest.Application.Events.Commands.CreateEvent;

public record CreateEventCommand(
    string Title,
    string Description,
    DateTime When,
    Location Where,
    int MaxParticipants) : IRequest<CreateEventResult>;