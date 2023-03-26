namespace BeMyGuest.Domain.Events;

public interface IEventRepository
{
    Task<bool> Add(Event @event);
}