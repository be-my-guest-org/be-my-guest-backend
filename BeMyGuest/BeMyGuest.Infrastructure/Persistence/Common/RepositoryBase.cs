using Amazon.DynamoDBv2;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace BeMyGuest.Infrastructure.Persistence.Common;

public abstract class RepositoryBase
{
    protected readonly IAmazonDynamoDB _dynamoDb;
    protected readonly DynamoDbOptions _dynamoDbOptions;

    public RepositoryBase(
        IAmazonDynamoDB dynamoDb,
        IOptions<DynamoDbOptions> options)
    {
        _dynamoDb = dynamoDb;
        _dynamoDbOptions = options.Value;
    }

    protected static string ToTableKey(string keyIdentifier, string value)
    {
        return $"{keyIdentifier}{KeyIdentifiers.Separator}{value}";
    }
}