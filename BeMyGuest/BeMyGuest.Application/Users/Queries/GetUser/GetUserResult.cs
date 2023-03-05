using BeMyGuest.Domain.Users;
using OneOf;
using OneOf.Types;

namespace BeMyGuest.Application.Users.Queries.GetUser;

[GenerateOneOf]
public partial class GetUserResult : OneOfBase<User, NotFound>
{
}