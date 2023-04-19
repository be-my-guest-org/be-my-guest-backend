using BeMyGuest.Common.Identifiers;
using BeMyGuest.Common.User;
using BeMyGuest.Domain.Events;
using BeMyGuest.Domain.GeoData;
using MediatR;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly CurrentUserData _currentUserData;
    private readonly IEventRepository _eventRepository;
    private readonly IGeoDataRepository _geoDataRepository;

    public CreateEventCommandHandler(IEventRepository eventRepository, IGeoDataRepository geoDataRepository, CurrentUserData currentUserData)
    {
        _eventRepository = eventRepository;
        _geoDataRepository = geoDataRepository;
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

        var geoPointAdded = await _geoDataRepository.Add(@event.Where.Coordinates, KeyIdentifiers.Event, @event.Id);

        if (!geoPointAdded)
        {
            return new Error();
        }

        return @event;
    }
}