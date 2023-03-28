using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Common.User;
using BeMyGuest.Domain.Users;
using BeMyGuest.Infrastructure.Configuration;
using BeMyGuest.Infrastructure.Persistence.Common;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeMyGuest.Infrastructure.Persistence.Users;

public class UserRepository : RepositoryBase, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        ILogger<UserRepository> logger,
        IAmazonDynamoDB dynamoDb,
        IOptions<DynamoDbOptions> options)
        : base(dynamoDb, options)
    {
        _logger = logger;
    }

    public async Task<User?> GetUser(CurrentUserData currentUserData)
    {
        _logger.LogInformation("GetUser {UserId}, Username: {Username}", currentUserData.UserId, currentUserData.Username);

        var getItemRequest = new GetItemRequest
        {
            TableName = _dynamoDbOptions.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = ToTableKey(KeyIdentifiers.User, currentUserData.UserId) } },
                { "sk", new AttributeValue { S = ToTableKey(KeyIdentifiers.Profile, currentUserData.Username) } },
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
}