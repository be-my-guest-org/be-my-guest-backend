using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Common.Utils;
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

    private string TableName => _dynamoDbOptions.TableName;

    public async Task<Event?> Get(Guid userId, Guid eventId)
    {
        _logger.LogInformation("Get event UserId: {UserId}, EventId: {EventId}", userId, eventId);

        var eventAsHostQueryRequest = EventQueryRequest(userId, eventId, KeyIdentifiers.EventHost);
        var eventAsHostResponse = await _dynamoDb.QueryAsync(eventAsHostQueryRequest);

        var eventAsGuestQueryRequest = EventQueryRequest(userId, eventId, KeyIdentifiers.EventGuest);
        var eventAsGuestResponse = await _dynamoDb.QueryAsync(eventAsGuestQueryRequest);

        if (eventAsHostResponse.Items.Any())
        {
            return eventAsHostResponse.Items.Select(ToDomainEventModel).Single();
        }

        if (eventAsGuestResponse.Items.Any())
        {
            return eventAsGuestResponse.Items.Select(ToDomainEventModel).Single();
        }

        return null;
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

        return response.Items.Select(ToDomainEventModel);
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

    public async Task<bool> UpdateGuests(Guid hostId, Guid guestId, Guid eventId, IEnumerable<Guid> guestIds)
    {
        var guestIdsAsAttributes = guestIds.Select(id => new AttributeValue { S = id.ToString() }).ToList();

        var updateRequest = new UpdateItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = hostId.PrependKeyIdentifiers(KeyIdentifiers.User) } },
                { "sk", new AttributeValue { S = eventId.PrependKeyIdentifiers(KeyIdentifiers.EventHost) } },
            },
            UpdateExpression = "SET #guestIds = :guestIdsNewValue, #updatedAt = :updatedAtNewValue",
            ExpressionAttributeNames = new Dictionary<string, string> { { "#guestIds", "guestIds" }, { "#updatedAt", "updatedAt" }, },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":guestIdsNewValue", new AttributeValue { L = guestIdsAsAttributes } },
                { ":updatedAtNewValue", new AttributeValue { S = DateTime.UtcNow.ToString("O") } },
            },
        };

        var response = await _dynamoDb.UpdateItemAsync(updateRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    private static Event ToDomainEventModel(Dictionary<string, AttributeValue> item)
    {
        var json = Document.FromAttributeMap(item).ToJson();

        var eventSnapshot = JsonSerializer.Deserialize<EventSnapshot>(json)!;

        return eventSnapshot.Adapt<Event>();
    }

    private QueryRequest EventQueryRequest(Guid userId, Guid eventId, string hostOrGuestKeyIdentifier)
    {
        return new QueryRequest
        {
            TableName = _dynamoDbOptions.TableName,
            KeyConditionExpression = "pk = :pk AND begins_with(sk, :sk)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":pk", new AttributeValue { S = userId.PrependKeyIdentifiers(KeyIdentifiers.User) } },
                { ":sk", new AttributeValue { S = eventId.PrependKeyIdentifiers(hostOrGuestKeyIdentifier) } },
            },
        };
    }
}