using BeMyGuest.Common.Common;

namespace BeMyGuest.Contracts.Events.Create;

public record CreateEventResponse(
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