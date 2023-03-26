using BeMyGuest.Common.Common;

namespace BeMyGuest.Contracts.Events;

public record CreateEventRequest(
    string Title,
    string Description,
    DateTime When,
    Location Where,
    int MaxParticipants);