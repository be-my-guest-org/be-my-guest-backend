using BeMyGuest.Domain.Events;
using OneOf;

namespace BeMyGuest.Application.Events.Queries.GetAllCurrentUserEvents;

[GenerateOneOf]
public partial class GetAllCurrentUserEventsResult : OneOfBase<List<Event>>
{
}