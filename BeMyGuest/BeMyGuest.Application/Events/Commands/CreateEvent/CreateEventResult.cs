using BeMyGuest.Domain.Events;
using OneOf;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Commands.CreateEvent;

[GenerateOneOf]
public partial class CreateEventResult : OneOfBase<Event, Error>
{
}