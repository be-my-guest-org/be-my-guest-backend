namespace BeMyGuest.Domain.Events.ValueObjects;

public class Status
{
    private const string OpenValue = "Open";
    private const string ClosedValue = "Closed";
    private const string FullValue = "Full";
    private const string ExpiredValue = "Expired";

    private Status(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Status Open() => new Status(OpenValue);

    public static Status Closed() => new Status(ClosedValue);

    public static Status Full() => new Status(FullValue);

    public static Status Expired() => new Status(ExpiredValue);

    public static Status From(string status)
    {
        return new Status(status);
    }
}