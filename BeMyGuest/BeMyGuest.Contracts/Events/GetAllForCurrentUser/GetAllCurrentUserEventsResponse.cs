using BeMyGuest.Contracts.Events.Get;

namespace BeMyGuest.Contracts.Events.GetAllForCurrentUser;

public record GetAllCurrentUserEventsResponse(IEnumerable<GetEventResponse> Events);