namespace BeMyGuest.Common.Utils;

public static class GuidExtensions
{
    public static string PrependKeyIdentifiers(this Guid value, params string[] identifiers)
    {
        return value.ToString().PrependKeyIdentifiers(identifiers);
    }

    public static string RemoveKeyIdentifiers(this Guid value)
    {
        return value.ToString().RemoveKeyIdentifiers();
    }
}