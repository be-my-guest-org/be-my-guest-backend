using System.Text.Json.Serialization;
using BeMyGuest.Infrastructure.Persistence.Common;

namespace BeMyGuest.Infrastructure.Persistence.Events;

public record EventParticipantSnapshot(
    [property: JsonPropertyName("gsi1pk")]
    string EventId,
    [property: JsonPropertyName("sk")]
    string EventParticipantId,
    [property: JsonPropertyName("role")]
    string Role,
    [property: JsonPropertyName("gsi2pk")]
    string UserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt = null) : SnapshotBase(CreatedAt, UpdatedAt);