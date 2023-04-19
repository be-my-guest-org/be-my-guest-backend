using BeMyGuest.Domain.Events;
using BeMyGuest.Domain.GeoData;
using MediatR;

namespace BeMyGuest.Application.Events.Queries.GetInRadius;

public class GetEventsInRadiusQueryHandler : IRequestHandler<GetEventsInRadiusQuery, GetEventsInRadiusResult>
{
    private readonly IEventRepository _eventRepository;
    private readonly IGeoDataRepository _geoDataRepository;

    public GetEventsInRadiusQueryHandler(IEventRepository eventRepository, IGeoDataRepository geoDataRepository)
    {
        _eventRepository = eventRepository;
        _geoDataRepository = geoDataRepository;
    }

    public async Task<GetEventsInRadiusResult> Handle(GetEventsInRadiusQuery query, CancellationToken cancellationToken)
    {
        var eventIds = await _geoDataRepository.GetInRadius(query.Coordinates, query.RadiusInMeters);
        foreach (Guid eventId in eventIds)
        {
            Console.WriteLine($"EventId {eventId}");
        }

        var getEventsTasks = eventIds.Select(id => _eventRepository.Get(id));
        var events = await Task.WhenAll(getEventsTasks);

        return new GetEventsInRadiusResult(events.ToList());
    }
}