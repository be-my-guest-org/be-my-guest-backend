using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using BeMyGuest.Domain.Users;
using Mapster;

namespace BeMyGuest.Infrastructure.Persistence.Users;

public class UserRepository : IUserRepository
{
    private const string TableName = "BeMyGuestStack-BeMyGuestTable3A49B3E7-BF2NWOM56WMW";
    private const string UserIdentifier = "USER";
    private const string ProfileIdentifier = "PROFILE";
    private const string KeySeparator = "#";
    private readonly IAmazonDynamoDB _dynamoDb;

    public UserRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<User?> GetUser(string userId, string username)
    {
        await Task.CompletedTask;
        var getItemRequest = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = ToTableKey(UserIdentifier, userId) } },
                { "sk", new AttributeValue { S = ToTableKey(ProfileIdentifier, username) } },
            },
        };

        var response = await _dynamoDb.GetItemAsync(getItemRequest);

        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);

        var userDto = JsonSerializer.Deserialize<UserSnapshot>(itemAsDocument.ToJson())!;

        return userDto.Adapt<User>();
    }

    private static string ToTableKey(string keyIdentifier, string value)
    {
        return $"{keyIdentifier}{KeySeparator}{value}";
    }
}