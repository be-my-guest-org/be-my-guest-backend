using BeMyGuest.Domain.Events;
using MediatR;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IEventRepository _eventRepository;

    public CreateEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<CreateEventResult> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var @event = Event.Create(
            command.Title,
            command.Description,
            command.When,
            command.Where,
            command.MaxParticipants,
            command.Host,
            command.Guests);

        var added = await _eventRepository.Add(@event);

        if (!added)
        {
            return new Error();
        }

        return @event;
    }
}