namespace BeMyGuest.Domain.Events;

public interface IEventRepository
{
    Task<Event?> Get(Guid eventId);

    Task<IEnumerable<Event>> GetAll(Guid userId);

    Task<bool> Add(Event @event);

    Task<bool> UpdateGuests(Guid hostId, Guid guestId, Guid eventId, IEnumerable<Guid> guestIds);
}