using BeMyGuest.Common.Common;
using BeMyGuest.Domain.Common.Models;
using BeMyGuest.Domain.Events.ValueObjects;
using OneOf;
using OneOf.Types;

namespace BeMyGuest.Domain.Events;

public class Event : EntityBase<Guid>
{
    private readonly List<Guid> _guests;

    private Event(
        Guid id,
        string title,
        string description,
        DateTime when,
        Location where,
        int maxParticipants,
        Guid hostId,
        List<Guid> guests,
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
        _guests = guests;
        Status = status;
    }

    public string Title { get; }

    public string Description { get; }

    public DateTime When { get; }

    public Location Where { get; }

    public int MaxParticipants { get; }

    public Guid HostId { get; }

    public IReadOnlyCollection<Guid> Guests => _guests.AsReadOnly();

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
            new List<Guid>(),
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
        List<Guid> guests,
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
            guests,
            status,
            createdAt,
            updatedAt);
    }

    public OneOf<Success, TooManyGuests> AddGuest(Guid guestId)
    {
        if (_guests.Count >= MaxParticipants - 1)
        {
            return new TooManyGuests();
        }

        _guests.Add(guestId);

        return new Success();
    }
}