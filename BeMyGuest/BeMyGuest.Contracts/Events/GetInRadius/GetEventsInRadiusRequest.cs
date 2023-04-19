namespace BeMyGuest.Contracts.Events.GetInRadius;

public record GetEventsInRadiusRequest(double Longitude, double Latitude, int RadiusInMeters);