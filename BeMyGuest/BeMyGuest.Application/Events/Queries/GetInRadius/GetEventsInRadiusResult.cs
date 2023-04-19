using BeMyGuest.Domain.Events;
using OneOf;

namespace BeMyGuest.Application.Events.Queries.GetInRadius;

[GenerateOneOf]
public partial class GetEventsInRadiusResult : OneOfBase<List<Event>>
{
}