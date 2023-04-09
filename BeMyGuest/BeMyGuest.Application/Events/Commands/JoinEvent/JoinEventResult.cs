using BeMyGuest.Domain.Events;
using OneOf;
using OneOf.Types;

namespace BeMyGuest.Application.Events.Commands.JoinEvent;

[GenerateOneOf]
public partial class JoinEventResult : OneOfBase<Success, NotFound, TooManyGuests, GuestAlreadyJoined, Error>
{
}