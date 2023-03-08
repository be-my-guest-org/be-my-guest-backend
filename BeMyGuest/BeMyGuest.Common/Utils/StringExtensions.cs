namespace BeMyGuest.Common.Utils;

public static class StringExtensions
{
    private const string FieldSpecifier = "#";

    public static string RemoveFieldSpecifier(this string value)
    {
        if (!value.Contains(FieldSpecifier))
        {
            return value;
        }

        return value[(value.IndexOf(FieldSpecifier, StringComparison.InvariantCultureIgnoreCase) + 1)..];
    }
}