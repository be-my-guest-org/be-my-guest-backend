using BeMyGuest.Common.User;
using BeMyGuest.Domain.Events;
using MediatR;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Queries.GetEvent;

public class GetEventQueryHandler : IRequestHandler<GetEventQuery, GetEventResult>
{
    private readonly CurrentUserData _currentUserData;
    private readonly IEventRepository _eventRepository;

    public GetEventQueryHandler(IEventRepository eventRepository, CurrentUserData currentUserData)
    {
        _eventRepository = eventRepository;
        _currentUserData = currentUserData;
    }

    public async Task<GetEventResult> Handle(GetEventQuery query, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.Get(_currentUserData.UserId, query.EventId);

        if (@event is null)
        {
            return new NotFound();
        }

        return @event;
    }
}