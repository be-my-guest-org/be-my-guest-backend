using System.Net;
using Amazon.DynamoDBv2.Model;
using Amazon.Geo;
using Amazon.Geo.Model;
using BeMyGuest.Common.Common;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Common.Utils;
using BeMyGuest.Domain.GeoData;
using Microsoft.Extensions.Logging;

namespace BeMyGuest.Infrastructure.Persistence.Geodata;

public class GeoDataRepository : IGeoDataRepository
{
    private readonly GeoDataManager _geoDataManager;
    private readonly ILogger<GeoDataRepository> _logger;

    public GeoDataRepository(GeoDataManager geoDataManager, ILogger<GeoDataRepository> logger)
    {
        _geoDataManager = geoDataManager;
        _logger = logger;
    }

    public async Task<IEnumerable<Guid>> GetInRadius(Coordinates coordinates, double radiusInMeters)
    {
        _logger.LogInformation(
            "GetInRadius latitude: {Latitude}, longitude: {Longitude}, radiusInMeters: {Radius}",
            coordinates.Latitude,
            coordinates.Longitude,
            radiusInMeters);

        var geoPoint = new GeoPoint(coordinates.Latitude, coordinates.Longitude);
        var request = new QueryRadiusRequest(geoPoint, radiusInMeters);

        var result = await _geoDataManager.QueryRadiusAsync(request);

        return result.Items.Select(values => Guid.Parse(values[KeyIdentifiers.Sk].S.RemoveKeyIdentifiers()));
    }

    public async Task<bool> Add(Coordinates coordinates, string type, Guid id)
    {
        _logger.LogInformation(
            "Add coordinates: {Coordinates}, type: {Type}, ID: {Id}",
            coordinates,
            type,
            id);

        var geoPoint = new GeoPoint(coordinates.Latitude, coordinates.Longitude);
        var putPointRequest = new PutPointRequest(geoPoint, new AttributeValue { S = id.PrependKeyIdentifiers(type) });

        var result = await _geoDataManager.PutPointAsync(putPointRequest);

        return result.PutItemResult.HttpStatusCode == HttpStatusCode.OK;
    }
}