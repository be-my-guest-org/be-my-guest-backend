using BeMyGuest.Common.Common;
using MediatR;

namespace BeMyGuest.Application.Events.Commands.CreateEvent;

public record CreateEventCommand(
    string Title,
    string Description,
    DateTime When,
    Location Where,
    int MaxParticipants,
    string Host,
    List<string> Guests) : IRequest<CreateEventResult>;