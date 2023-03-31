using BeMyGuest.Common.User;
using BeMyGuest.Domain.Events;
using MediatR;

namespace BeMyGuest.Application.Events.Queries.GetAllCurrentUserEvents;

public class GetAllCurrentUserEventsQueryHandler : IRequestHandler<GetAllCurrentUserEventsQuery, GetAllCurrentUserEventsResult>
{
    private readonly CurrentUserData _currentUserData;
    private readonly IEventRepository _eventRepository;

    public GetAllCurrentUserEventsQueryHandler(IEventRepository eventRepository, CurrentUserData currentUserData)
    {
        _eventRepository = eventRepository;
        _currentUserData = currentUserData;
    }

    public async Task<GetAllCurrentUserEventsResult> Handle(GetAllCurrentUserEventsQuery query, CancellationToken cancellationToken)
    {
        var events = (await _eventRepository.GetAll(_currentUserData.UserId)).ToList();
        return events;
    }
}