using BeMyGuest.Domain.Events;
using MediatR;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Queries.GetEvent;

public class GetEventQueryHandler : IRequestHandler<GetEventQuery, GetEventResult>
{
    private readonly IEventRepository _eventRepository;

    public GetEventQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<GetEventResult> Handle(GetEventQuery query, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.Get(query.EventId);

        if (@event is null)
        {
            return new NotFound();
        }

        return @event;
    }
}