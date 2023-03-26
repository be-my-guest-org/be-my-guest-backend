using BeMyGuest.Common.Common;

namespace BeMyGuest.Domain.Events;

public class Event
{
    private Event(
        Guid id,
        string title,
        string description,
        DateTime when,
        Location where,
        int maxParticipants,
        string hostId,
        List<string> guests)
    {
        Id = id;
        Title = title;
        Description = description;
        When = when;
        Where = where;
        MaxParticipants = maxParticipants;
        HostId = hostId;
        Guests = guests;
    }

    public Guid Id { get; }

    public string Title { get; }

    public string Description { get; }

    public DateTime When { get; }

    public Location Where { get; }

    public int MaxParticipants { get; }

    public string HostId { get; }

    public List<string> Guests { get; }

    public static Event Create(
        string title,
        string description,
        DateTime when,
        Location where,
        int maxParticipants,
        string hostId)
    {
        return new Event(
            Guid.NewGuid(),
            title,
            description,
            when,
            where,
            maxParticipants,
            hostId,
            new List<string>());
    }
}