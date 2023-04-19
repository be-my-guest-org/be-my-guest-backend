using System.Net;
using Amazon.DynamoDBv2.Model;
using Amazon.Geo;
using Amazon.Geo.Model;
using BeMyGuest.Common.Common;
using BeMyGuest.Common.Utils;
using BeMyGuest.Domain.GeoData;

namespace BeMyGuest.Infrastructure.Persistence.Geodata;

public class GeoDataRepository : IGeoDataRepository
{
    private readonly GeoDataManager _geoDataManager;

    public GeoDataRepository(GeoDataManager geoDataManager)
    {
        _geoDataManager = geoDataManager;
    }

    public async Task<bool> Add(Coordinates coordinates, string type, Guid id)
    {
        var geoPoint = new GeoPoint(coordinates.Latitude, coordinates.Longitude);
        var putPointRequest = new PutPointRequest(geoPoint, new AttributeValue { S = id.PrependKeyIdentifiers(type) });

        var result = await _geoDataManager.PutPointAsync(putPointRequest);

        return result.PutItemResult.HttpStatusCode == HttpStatusCode.OK;
    }
}