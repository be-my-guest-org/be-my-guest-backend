using BeMyGuest.Common.Common;

namespace BeMyGuest.Domain.GeoData;

public interface IGeoDataRepository
{
    Task<bool> Add(Coordinates coordinates, string type, Guid id);
}