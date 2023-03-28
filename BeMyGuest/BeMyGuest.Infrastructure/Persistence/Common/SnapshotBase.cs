using System.Text.Json.Serialization;

namespace BeMyGuest.Infrastructure.Persistence.Common;

public abstract record SnapshotBase(
    [property: JsonPropertyName("createdAt")]
    DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")]
    DateTime? UpdatedAt = null);