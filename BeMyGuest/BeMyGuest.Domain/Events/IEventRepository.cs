namespace BeMyGuest.Domain.Events;

public interface IEventRepository
{
    Task<Event?> Get(Guid eventId);

    Task<IEnumerable<Event>> GetAll(Guid userId);

    Task<bool> Add(Event @event);

    Task<bool> Join(Guid eventId, Guid guestId);
}