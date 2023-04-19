using BeMyGuest.Common.Common;

namespace BeMyGuest.Domain.GeoData;

public interface IGeoDataRepository
{
    Task<IEnumerable<Guid>> GetInRadius(Coordinates coordinates, double radiusInMeters);

    Task<bool> Add(Coordinates coordinates, string type, Guid id);
}