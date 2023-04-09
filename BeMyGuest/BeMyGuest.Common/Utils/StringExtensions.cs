using System.Text;
using BeMyGuest.Common.Identifiers;

namespace BeMyGuest.Common.Utils;

public static class StringExtensions
{
    public static string PrependKeyIdentifiers(this string value, params string[] identifiers)
    {
        if (!identifiers.Any())
        {
            return value;
        }

        var stringBuilder = new StringBuilder();

        foreach (string identifier in identifiers)
        {
            stringBuilder
                .Append(identifier)
                .Append(KeyIdentifiers.Separator);
        }

        stringBuilder.Append(value);

        return stringBuilder.ToString();
    }

    public static string RemoveKeyIdentifiers(this string value)
    {
        if (!value.Contains(KeyIdentifiers.Separator))
        {
            return value;
        }

        return value[(value.LastIndexOf(KeyIdentifiers.Separator, StringComparison.InvariantCultureIgnoreCase) + 1)..];
    }
}