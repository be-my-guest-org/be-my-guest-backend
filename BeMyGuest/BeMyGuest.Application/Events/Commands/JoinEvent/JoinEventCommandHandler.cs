using BeMyGuest.Common.User;
using BeMyGuest.Domain.Events;
using MediatR;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Commands.JoinEvent;

public class JoinEventCommandHandler : IRequestHandler<JoinEventCommand, JoinEventResult>
{
    private readonly CurrentUserData _currentUserData;
    private readonly IEventRepository _eventRepository;

    public JoinEventCommandHandler(IEventRepository eventRepository, CurrentUserData currentUserData)
    {
        _eventRepository = eventRepository;
        _currentUserData = currentUserData;
    }

    public async Task<JoinEventResult> Handle(JoinEventCommand command, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.Get(command.EventId);

        if (@event is null)
        {
            return new NotFound();
        }

        var result = @event.AddGuest(_currentUserData.UserId);

        if (result.IsT1)
        {
            return result.AsT1;
        }

        if (result.IsT2)
        {
            return result.AsT2;
        }

        var updateGuestResult = await _eventRepository.Join(
            @event.Id,
            command.GuestId);

        if (!updateGuestResult)
        {
            return new Error();
        }

        return new Success();
    }
}