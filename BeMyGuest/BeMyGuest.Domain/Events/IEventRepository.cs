namespace BeMyGuest.Domain.Events;

public interface IEventRepository
{
    Task<Event?> Get(Guid userId, Guid eventId);

    Task<bool> Add(Event @event);
}