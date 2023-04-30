namespace BeMyGuest.Common.Utils;

public static class NumericIdGenerator
{
    public static long Generate()
    {
        var random = new Random();
        return random.NextInt64();
    }
}