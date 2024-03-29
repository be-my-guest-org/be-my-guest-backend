﻿using System.Text.Json.Serialization;
using BeMyGuest.Infrastructure.Persistence.Common;

namespace BeMyGuest.Infrastructure.Persistence.Users;

public record UserSnapshot(
    [property: JsonPropertyName("pk")]
    string Pk,
    [property: JsonPropertyName("sk")]
    string Sk,
    [property: JsonPropertyName("firstName")]
    string FirstName,
    [property: JsonPropertyName("lastName")]
    string LastName,
    [property: JsonPropertyName("email")]
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt = null
) : SnapshotBase(CreatedAt, UpdatedAt);