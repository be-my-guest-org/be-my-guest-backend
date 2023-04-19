using BeMyGuest.Contracts.Events.Get;

namespace BeMyGuest.Contracts.Events.GetInRadius;

public record GetEventsInRadiusResponse(IEnumerable<GetEventResponse> Events);