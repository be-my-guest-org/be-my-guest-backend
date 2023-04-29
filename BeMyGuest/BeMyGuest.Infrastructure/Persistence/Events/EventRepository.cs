﻿using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Geo;
using Amazon.Geo.Model;
using BeMyGuest.Common.Common;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Common.Utils;
using BeMyGuest.Domain.Events;
using BeMyGuest.Domain.Events.ValueObjects;
using BeMyGuest.Infrastructure.Configuration;
using BeMyGuest.Infrastructure.Persistence.Common;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeMyGuest.Infrastructure.Persistence.Events;

public class EventRepository : RepositoryBase, IEventRepository
{
    private readonly GeoDataManager _geoDataManager;
    private readonly ILogger<EventRepository> _logger;

    public EventRepository(
        ILogger<EventRepository> logger,
        IAmazonDynamoDB dynamoDb,
        IOptions<DynamoDbOptions> options,
        GeoDataManager geoDataManager)
        : base(dynamoDb, options)
    {
        _logger = logger;
        _geoDataManager = geoDataManager;
    }

    private string TableName => _dynamoDbOptions.TableName;

    private string Gsi1Name => _dynamoDbOptions.Gsi1Name;

    private string Gsi2Name => _dynamoDbOptions.Gsi2Name;

    public async Task<Event?> Get(Guid eventId)
    {
        _logger.LogInformation("Get event EventId: {EventId}", eventId);

        var request = new QueryRequest
        {
            TableName = TableName,
            IndexName = Gsi1Name,
            KeyConditionExpression = "gsi1pk = :pk",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":pk", new AttributeValue { S = eventId.PrependKeyIdentifiers(KeyIdentifiers.Event) } },
            },
        };

        var response = await _dynamoDb.QueryAsync(request);

        if (!response.Items.Any())
        {
            return null;
        }

        var eventSnapshot = ToSnapshot<EventDataSnapshot>(response.Items.Single(item => item["sk"].S.StartsWith(KeyIdentifiers.EventData)));

        var participantsSnapshot = response.Items
            .Where(item => !item["sk"].S.StartsWith(KeyIdentifiers.EventData))
            .Select(ToSnapshot<EventParticipantSnapshot>);

        return (eventSnapshot, participantsSnapshot).Adapt<Event>();
    }

    public async Task<IEnumerable<Event>> GetAll(Guid userId)
    {
        _logger.LogInformation("Get all events for user UserId {UserId}, GsiName: {GsiName}", userId, Gsi1Name);

        var queryRequest = new QueryRequest
        {
            TableName = TableName,
            IndexName = Gsi2Name,
            KeyConditionExpression = "gsi2pk = :pk AND begins_with(sk, :sk)",
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

        var putPointRequest = CreatePutPointRequest(@event, eventAsAttributes);

        var result = await _geoDataManager.PutPointAsync(putPointRequest);

        if (result.PutItemResult.HttpStatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        return await AddParticipant(@event, @event.HostId, ParticipantRoles.Host);
    }

    public async Task<bool> Join(Event @event, Guid guestId)
    {
        _logger.LogInformation("Join event EventId: {EventId}, GuestId: {GuestId}", @event.Id, guestId);

        return await AddParticipant(@event, guestId, ParticipantRoles.Guest);
    }

    public async Task<bool> UpdateStatus(Guid eventId, Status status)
    {
        _logger.LogInformation("UpdateStatus event EventId: {EventId}, new status: {Status}", eventId, status.Value);

        var key = new Dictionary<string, AttributeValue>
        {
            { "pk", new AttributeValue { S = eventId.PrependKeyIdentifiers(KeyIdentifiers.Event) } },
            { "sk", new AttributeValue { S = KeyIdentifiers.EventData, } },
        };

        var updates = new Dictionary<string, AttributeValueUpdate>
        {
            { "status", new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { S = status.Value } } },
        };

        var request = new UpdateItemRequest { TableName = TableName, Key = key, AttributeUpdates = updates };

        var response = await _dynamoDb.UpdateItemAsync(request);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    private static T ToSnapshot<T>(Dictionary<string, AttributeValue> item)
    {
        var json = Document.FromAttributeMap(item).ToJson();
        var eventSnapshot = JsonSerializer.Deserialize<T>(json)!;

        return eventSnapshot;
    }

    private async Task<bool> AddParticipant(Event @event, Guid userId, string role)
    {
        var eventSnapshot = (@event.Id, userId, role).Adapt<EventParticipantSnapshot>();
        var eventAsJson = JsonSerializer.Serialize(eventSnapshot);
        var eventAsAttributes = Document.FromJson(eventAsJson).ToAttributeMap();

        var putPointRequest = CreatePutPointRequest(@event, eventAsAttributes);
        var response = await _geoDataManager.PutPointAsync(putPointRequest);

        return response.PutItemResult.HttpStatusCode == HttpStatusCode.OK;
    }

    private static PutPointRequest CreatePutPointRequest(Event @event, Dictionary<string, AttributeValue> item)
    {
        var geoPoint = new GeoPoint(@event.Where.Coordinates.Latitude, @event.Where.Coordinates.Longitude);
        var putPointRequest = new PutPointRequest(geoPoint, new AttributeValue { S = @event.Id.PrependKeyIdentifiers(KeyIdentifiers.EventData) })
        {
            PutItemRequest = { Item = item, ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)" },
        };
        return putPointRequest;
    }
}