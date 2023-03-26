using BeMyGuest.Common.Common;
using BeMyGuest.Domain.Common.Models;

namespace BeMyGuest.Domain.Events;

public class Event : EntityBase<Guid>
{
    private Event(
        Guid id,
        string title,
        string description,
        DateTime when,
        Location where,
        int maxParticipants,
        string hostId,
        List<string> guests,
        DateTime createdAt,
        DateTime? updatedAt = null)
        : base(id, createdAt, updatedAt)
    {
        Title = title;
        Description = description;
        When = when;
        Where = where;
        MaxParticipants = maxParticipants;
        HostId = hostId;
        Guests = guests;
    }

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
            new List<string>(),
            DateTime.UtcNow);
    }
}