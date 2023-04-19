using BeMyGuest.Common.Common;
using MediatR;

namespace BeMyGuest.Application.Events.Queries.GetInRadius;

public record GetEventsInRadiusQuery(Coordinates Coordinates, double RadiusInMeters) : IRequest<GetEventsInRadiusResult>;