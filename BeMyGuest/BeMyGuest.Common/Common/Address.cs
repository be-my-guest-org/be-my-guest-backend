using System.Text.Json.Serialization;

namespace BeMyGuest.Common.Common;

public record Location(
    [property: JsonPropertyName("qualifier")]
    string Qualifier,
    [property: JsonPropertyName("address")]
    string Address,
    [property: JsonPropertyName("number")]
    string Number,
    [property: JsonPropertyName("city")]
    string City,
    [property: JsonPropertyName("cap")]
    string Cap,
    [property: JsonPropertyName("province")]
    string Province,
    [property: JsonPropertyName("country")]
    string Country);