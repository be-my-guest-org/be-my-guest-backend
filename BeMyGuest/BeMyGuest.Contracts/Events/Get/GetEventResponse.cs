using BeMyGuest.Common.Common;

namespace BeMyGuest.Contracts.Events.Get;

public record GetEventResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime When,
    Location Where,
    int MaxParticipants,
    string HostId,
    List<string> Guests,
    DateTime CreatedAt,
    DateTime? UpdatedAt);