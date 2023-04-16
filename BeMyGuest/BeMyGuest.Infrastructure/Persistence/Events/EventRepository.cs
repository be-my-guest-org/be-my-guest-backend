using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using BeMyGuest.Common.Common;
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

    private string Gsi1Name => _dynamoDbOptions.Gsi1Name;

    public async Task<Event?> Get(Guid eventId)
    {
        _logger.LogInformation("Get event EventId: {EventId}", eventId);

        var request = new QueryRequest
        {
            TableName = _dynamoDbOptions.TableName,
            KeyConditionExpression = "pk = :pk AND sk = :sk",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":pk", new AttributeValue { S = eventId.PrependKeyIdentifiers(KeyIdentifiers.Event) } },
                { ":sk", new AttributeValue { S = KeyIdentifiers.EventData, } },
            },
        };

        var response = await _dynamoDb.QueryAsync(request);

        if (!response.Items.Any())
        {
            return null;
        }

        var eventSnapshot = ToSnapshot<EventDataSnapshot>(response.Items.Single(item => item["sk"].S == KeyIdentifiers.EventData));

        var participantsSnapshot = response.Items
            .Where(item => item["sk"].S != KeyIdentifiers.EventData)
            .Select(ToSnapshot<EventParticipantSnapshot>);

        return (eventSnapshot, participantsSnapshot).Adapt<Event>();
    }

    public async Task<IEnumerable<Event>> GetAll(Guid userId)
    {
        var queryRequest = new QueryRequest
        {
            TableName = TableName,
            IndexName = Gsi1Name,
            KeyConditionExpression = "gsi1pk = :pk AND begins_with(sk, :sk)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":pk", new AttributeValue { S = userId.PrependKeyIdentifiers(KeyIdentifiers.User) } },
                { ":sk", new AttributeValue { S = KeyIdentifiers.Event } },
            },
        };

        var response = await _dynamoDb.QueryAsync(queryRequest);

        var getEventTasks = response.Items.Select(item => Get(Guid.Parse(item["pk"].S.RemoveKeyIdentifiers())));
        var events = await Task.WhenAll(getEventTasks);

        return events.Where(e => e != null).Cast<Event>();
    }

    public async Task<bool> Add(Event @event)
    {
        _logger.LogInformation("Add event Title: {Title}, Description: {Description}", @event.Title, @event.Description);

        var eventSnapshot = @event.Adapt<EventDataSnapshot>();
        var eventAsJson = JsonSerializer.Serialize(eventSnapshot);
        var eventAsAttributes = Document.FromJson(eventAsJson).ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _dynamoDbOptions.TableName, Item = eventAsAttributes, ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)",
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> Join(Guid eventId, Guid guestId)
    {
        _logger.LogInformation("Join event EventId: {EventId}, GuestId: {GuestId}", eventId, guestId);

        var eventSnapshot = (eventId, guestId, ParticipantRoles.Guest).Adapt<EventParticipantSnapshot>();
        var eventAsJson = JsonSerializer.Serialize(eventSnapshot);
        var eventAsAttributes = Document.FromJson(eventAsJson).ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _dynamoDbOptions.TableName, Item = eventAsAttributes, ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)",
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    private static Event ToDomainEventModel(Dictionary<string, AttributeValue> item)
    {
        EventDataSnapshot eventDataSnapshot = ToEventDataSnapshot(item);

        return eventDataSnapshot.Adapt<Event>();
    }

    private static EventDataSnapshot ToEventDataSnapshot(Dictionary<string, AttributeValue> item)
    {
        var json = Document.FromAttributeMap(item).ToJson();
        var eventSnapshot = JsonSerializer.Deserialize<EventDataSnapshot>(json)!;

        return eventSnapshot;
    }

    private static T ToSnapshot<T>(Dictionary<string, AttributeValue> item)
    {
        var json = Document.FromAttributeMap(item).ToJson();
        var eventSnapshot = JsonSerializer.Deserialize<T>(json)!;

        return eventSnapshot;
    }
}