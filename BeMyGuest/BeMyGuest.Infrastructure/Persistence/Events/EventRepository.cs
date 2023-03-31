using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Domain.Events;
using BeMyGuest.Infrastructure.Configuration;
using BeMyGuest.Infrastructure.Persistence.Common;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeMyGuest.Infrastructure.Persistence.Events;

public class EventRepository : RepositoryBase, IEventRepository
{
    private readonly ILogger<EventRepository> _logger;

    public EventRepository(
        ILogger<EventRepository> logger,
        IAmazonDynamoDB dynamoDb,
        IOptions<DynamoDbOptions> options)
        : base(dynamoDb, options)
    {
        _logger = logger;
    }

    public async Task<Event?> Get(Guid userId, Guid eventId)
    {
        _logger.LogInformation("Get event UserId: {UserId}, EventId: {EventId}", userId, eventId);

        var getItemRequest = new GetItemRequest
        {
            TableName = _dynamoDbOptions.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = ToTableKey(KeyIdentifiers.User, userId.ToString()) } },
                { "sk", new AttributeValue { S = ToTableKey(KeyIdentifiers.Event, eventId.ToString()) } },
            },
        };

        var response = await _dynamoDb.GetItemAsync(getItemRequest);

        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);

        var eventSnapshot = JsonSerializer.Deserialize<EventSnapshot>(itemAsDocument.ToJson())!;

        return eventSnapshot.Adapt<Event>();
    }

    public async Task<IEnumerable<Event>> GetAll(Guid userId)
    {
        var queryRequest = new QueryRequest
        {
            TableName = _dynamoDbOptions.TableName,
            KeyConditionExpression = "pk = :pk AND begins_with(sk, :sk)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":pk", new AttributeValue { S = $"{KeyIdentifiers.User}{KeyIdentifiers.Separator}{userId}" } },
                { ":sk", new AttributeValue { S = KeyIdentifiers.Event } },
            },
        };

        var response = await _dynamoDb.QueryAsync(queryRequest);

        return response.Items.Select(item =>
        {
            var json = Document.FromAttributeMap(item).ToJson();

            var eventSnapshot = JsonSerializer.Deserialize<EventSnapshot>(json)!;

            return eventSnapshot.Adapt<Event>();
        });
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