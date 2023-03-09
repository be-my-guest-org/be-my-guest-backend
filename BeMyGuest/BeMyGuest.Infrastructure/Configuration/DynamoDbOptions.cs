namespace BeMyGuest.Infrastructure.Configuration;

public class DynamoDbOptions
{
    public const string Section = "DynamoDb";

    public string TableName { get; set; } = null!;
}