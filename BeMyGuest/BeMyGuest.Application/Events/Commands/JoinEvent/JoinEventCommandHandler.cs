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

        var oldStatus = @event.Status;
        var result = @event.AddGuest(_currentUserData.UserId);

        // TooManyGuests
        if (result.IsT1)
        {
            return result.AsT1;
        }

        // Already joined
        if (result.IsT2)
        {
            return result.AsT2;
        }

        var updateGuestResult = await _eventRepository.Join(
            @event.Id,
            _currentUserData.UserId);

        bool updateStatusResult = true;
        if (@event.Status != oldStatus)
        {
            updateStatusResult = await _eventRepository.UpdateStatus(@event.Id, @event.Status);
        }

        if (!updateGuestResult || !updateStatusResult)
        {
            return new Error();
        }

        return new Success();
    }
}