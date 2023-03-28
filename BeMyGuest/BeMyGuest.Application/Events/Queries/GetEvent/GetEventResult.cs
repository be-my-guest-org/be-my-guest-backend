using BeMyGuest.Domain.Events;
using OneOf;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Queries.GetEvent;

[GenerateOneOf]
public partial class GetEventResult : OneOfBase<Event, NotFound>
{
}