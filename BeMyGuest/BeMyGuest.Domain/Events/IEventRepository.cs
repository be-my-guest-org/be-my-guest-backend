using BeMyGuest.Domain.Events.ValueObjects;

namespace BeMyGuest.Domain.Events;

public interface IEventRepository
{
    Task<Event?> Get(Guid eventId);

    Task<IEnumerable<Event>> GetAll(Guid userId);

    Task<bool> Add(Event @event);

    Task<bool> Join(Event @event, Guid guestId);

    Task<bool> UpdateStatus(Guid eventId, Status eventStatus);
}