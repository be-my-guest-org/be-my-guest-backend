using BeMyGuest.Common.User;
using BeMyGuest.Domain.Events;
using MediatR;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly CurrentUserData _currentUserData;
    private readonly IEventRepository _eventRepository;

    public CreateEventCommandHandler(IEventRepository eventRepository, CurrentUserData currentUserData)
    {
        _eventRepository = eventRepository;
        _currentUserData = currentUserData;
    }

    public async Task<CreateEventResult> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var @event = Event.Create(
            command.Title,
            command.Description,
            command.When,
            command.Where,
            command.MaxParticipants,
            _currentUserData.UserId);

        var added = await _eventRepository.Add(@event);

        if (!added)
        {
            return new Error();
        }

        return @event;
    }
}