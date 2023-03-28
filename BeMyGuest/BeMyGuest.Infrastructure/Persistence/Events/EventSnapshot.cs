using System.Text.Json.Serialization;
using BeMyGuest.Common.Common;

namespace BeMyGuest.Infrastructure.Persistence.Events;

public record EventSnapshot(
    [property: JsonPropertyName("pk")]
    string HostId,
    [property: JsonPropertyName("sk")]
    string EventId,
    [property: JsonPropertyName("title")]
    string Title,
    [property: JsonPropertyName("description")]
    string Description,
    [property: JsonPropertyName("when")]
    DateTime When,
    [property: JsonPropertyName("where")]
    Location Where,
    [property: JsonPropertyName("maxParticipants")]
    int MaxParticipants,
    [property: JsonPropertyName("guestIds")]
    List<string> Guests,
    DateTime CreatedAt,
    DateTime? UpdatedAt = null) : SnapshotBase(CreatedAt, UpdatedAt);