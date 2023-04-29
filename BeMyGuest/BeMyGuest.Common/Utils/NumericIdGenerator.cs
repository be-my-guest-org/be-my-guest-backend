using System.Numerics;
using System.Text.RegularExpressions;

namespace BeMyGuest.Common.Utils;

public static class NumericIdGenerator
{
    public static BigInteger Generate()
    {
        var guid = Guid.NewGuid();
        return BigInteger.Parse(Regex.Replace(guid.ToString(), "[^0-9]", string.Empty));
    }
}