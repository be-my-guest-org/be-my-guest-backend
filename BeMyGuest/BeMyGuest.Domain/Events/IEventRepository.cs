namespace BeMyGuest.Domain.Events;

public interface IEventRepository
{
    Task<Event?> Get(Guid userId, Guid eventId);

    Task<IEnumerable<Event>> GetAll(Guid userId);

    Task<bool> Add(Event @event);
}