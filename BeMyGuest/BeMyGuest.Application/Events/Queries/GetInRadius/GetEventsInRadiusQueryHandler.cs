using BeMyGuest.Domain.Events;
using MediatR;

namespace BeMyGuest.Application.Events.Queries.GetInRadius;

public class GetEventsInRadiusQueryHandler : IRequestHandler<GetEventsInRadiusQuery, GetEventsInRadiusResult>
{
    private readonly IEventRepository _eventRepository;

    public GetEventsInRadiusQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<GetEventsInRadiusResult> Handle(GetEventsInRadiusQuery query, CancellationToken cancellationToken)
    {
        var events = await _eventRepository.GetInRadius(query.Coordinates, query.RadiusInMeters);

        return new GetEventsInRadiusResult(events.ToList());
    }
}