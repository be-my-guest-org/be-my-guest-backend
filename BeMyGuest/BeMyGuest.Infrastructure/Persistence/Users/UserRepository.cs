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
    private readonly CurrentUserData _currentUserData;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        ILogger<UserRepository> logger,
        IAmazonDynamoDB dynamoDb,
        IOptions<DynamoDbOptions> options,
        CurrentUserData currentUserData)
        : base(dynamoDb, options)
    {
        _logger = logger;
        _currentUserData = currentUserData;
    }

    public async Task<User?> GetUser()
    {
        _logger.LogInformation("GetUser {UserId}, Username: {Username}", _currentUserData.UserId, _currentUserData.Username);

        var getItemRequest = new GetItemRequest
        {
            TableName = _dynamoDbOptions.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = ToTableKey(KeyIdentifiers.User, _currentUserData.UserId) } },
                { "sk", new AttributeValue { S = ToTableKey(KeyIdentifiers.Profile, _currentUserData.Username) } },
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