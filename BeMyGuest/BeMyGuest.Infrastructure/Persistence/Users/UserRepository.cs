﻿using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using BeMyGuest.Common.User;
using BeMyGuest.Domain.Users;
using BeMyGuest.Infrastructure.Configuration;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeMyGuest.Infrastructure.Persistence.Users;

public class UserRepository : IUserRepository
{
    private const string UserIdentifier = "USER";
    private const string ProfileIdentifier = "PROFILE";
    private const string KeySeparator = "#";
    private readonly CurrentUserData _currentUserData;
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly DynamoDbOptions _dynamoDbOptions;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        ILogger<UserRepository> logger,
        IAmazonDynamoDB dynamoDb,
        IOptions<DynamoDbOptions> options,
        CurrentUserData currentUserData)
    {
        _logger = logger;
        _currentUserData = currentUserData;
        _dynamoDb = dynamoDb;
        _dynamoDbOptions = options.Value;
    }

    public async Task<User?> GetUser()
    {
        _logger.LogInformation("GetUser {UserId}, Username: {Username}", _currentUserData.UserId, _currentUserData.Username);

        var getItemRequest = new GetItemRequest
        {
            TableName = _dynamoDbOptions.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = ToTableKey(UserIdentifier, _currentUserData.UserId) } },
                { "sk", new AttributeValue { S = ToTableKey(ProfileIdentifier, _currentUserData.Username) } },
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