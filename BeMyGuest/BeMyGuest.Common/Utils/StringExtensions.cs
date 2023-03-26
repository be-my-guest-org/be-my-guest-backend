using BeMyGuest.Common.Identifiers;

namespace BeMyGuest.Common.Utils;

public static class StringExtensions
{
    public static string PrependKeyIdentifier(this string value, string identifier)
    {
        return $"{identifier}{KeyIdentifiers.Separator}{value}";
    }

    public static string RemoveKeyIdentifier(this string value)
    {
        if (!value.Contains(KeyIdentifiers.Separator))
        {
            return value;
        }

        return value[(value.IndexOf(KeyIdentifiers.Separator, StringComparison.InvariantCultureIgnoreCase) + 1)..];
    }
}