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
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);