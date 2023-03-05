using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BeMyGuest.Domain.Users;
using Mapster;

namespace BeMyGuest.Infrastructure.Persistence.Users;

public class UserRepository : IUserRepository
{
    private const string TableName = "BeMyGuestStack-Tablebemyguesttable62D87A46-MW0F9MI42LK2";
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
                { "pk", new AttributeValue { S = userId } }, { "sk", new AttributeValue { S = username } },
            },
        };

        // var response = await _dynamoDb.GetItemAsync(getItemRequest);
        //
        // if (response.Item.Count == 0)
        // {
        //     return null;
        // }
        //
        // var itemAsDocument = Document.FromAttributeMap(response.Item);
        //
        // var userDto = JsonSerializer.Deserialize<UserDto>(itemAsDocument.ToJson())!;

        var userDto = new UserDto("a#aaa", "b#bbb", "fn", "ln", "email", DateTime.UtcNow, DateTime.UtcNow);

        return userDto.Adapt<User>();
    }
}