using BeMyGuest.Common.Identifiers;

namespace BeMyGuest.Infrastructure.Persistence.Common;

public abstract class RepositoryBase
{
    protected static string ToTableKey(string keyIdentifier, string value)
    {
        return $"{keyIdentifier}{KeyIdentifiers.Separator}{value}";
    }
}