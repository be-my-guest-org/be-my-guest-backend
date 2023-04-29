using System.Numerics;
using System.Text.Json.Serialization;
using BeMyGuest.Common.Common;
using BeMyGuest.Infrastructure.Persistence.Common;

namespace BeMyGuest.Infrastructure.Persistence.Events;

public record EventDataSnapshot(
    [property: JsonPropertyName("gsi1pk")]
    string EventId,
    [property: JsonPropertyName("pk")]
    BigInteger Pk,
    [property: JsonPropertyName("sk")]
    string EventData,
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
    [property: JsonPropertyName("status")]
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt = null) : SnapshotBase(CreatedAt, UpdatedAt);