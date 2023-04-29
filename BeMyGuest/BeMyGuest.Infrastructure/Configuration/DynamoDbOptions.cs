namespace BeMyGuest.Infrastructure.Configuration;

public class DynamoDbOptions
{
    public const string Section = "DynamoDb";

    public string TableName { get; set; } = null!;

    public string GeoDataTableName { get; set; } = null!;

    public string Gsi1Name { get; set; } = null!;

    public string Gsi2Name { get; set; } = null!;
}