using BeMyGuest.Common.Common;
using BeMyGuest.Domain.Common.Models;
using BeMyGuest.Domain.Events.ValueObjects;

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
        Guid hostId,
        List<string> guests,
        Status status,
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
        Status = status;
    }

    public string Title { get; }

    public string Description { get; }

    public DateTime When { get; }

    public Location Where { get; }

    public int MaxParticipants { get; }

    public Guid HostId { get; }

    public List<string> Guests { get; }

    public Status Status { get; }

    public static Event Create(
        string title,
        string description,
        DateTime when,
        Location where,
        int maxParticipants,
        Guid hostId)
    {
        return Create(
            Guid.NewGuid(),
            title,
            description,
            when,
            where,
            maxParticipants,
            hostId,
            Status.Open(),
            DateTime.UtcNow,
            null);
    }

    public static Event Create(
        Guid id,
        string title,
        string description,
        DateTime when,
        Location where,
        int maxParticipants,
        Guid hostId,
        Status status,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new Event(
            id,
            title,
            description,
            when,
            where,
            maxParticipants,
            hostId,
            new List<string>(),
            status,
            createdAt,
            updatedAt);
    }
}