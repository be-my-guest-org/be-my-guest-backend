namespace BeMyGuest.Contracts.Events.Join;

public record JoinEventRequest(Guid HostId, Guid EventId);