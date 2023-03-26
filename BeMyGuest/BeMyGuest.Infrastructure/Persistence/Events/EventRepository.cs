using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using BeMyGuest.Common.User;
using BeMyGuest.Domain.Events;
using BeMyGuest.Infrastructure.Configuration;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeMyGuest.Infrastructure.Persistence.Events;

public class EventRepository : IEventRepository
{
    private readonly CurrentUserData _currentUserData;
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly DynamoDbOptions _dynamoDbOptions;
    private readonly ILogger<EventRepository> _logger;

    public EventRepository(
        ILogger<EventRepository> logger,
        IAmazonDynamoDB dynamoDb,
        IOptions<DynamoDbOptions> options,
        CurrentUserData currentUserData)
    {
        _logger = logger;
        _currentUserData = currentUserData;
        _dynamoDb = dynamoDb;
        _dynamoDbOptions = options.Value;
    }

    public async Task<bool> Add(Event @event)
    {
        _logger.LogInformation("Add event Title: {Title}, Description: {Description}", @event.Title, @event.Description);

        var eventSnapshot = @event.Adapt<EventSnapshot>();
        var eventAsJson = JsonSerializer.Serialize(eventSnapshot);
        var eventAsAttributes = Document.FromJson(eventAsJson).ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _dynamoDbOptions.TableName, Item = eventAsAttributes, ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)",
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}