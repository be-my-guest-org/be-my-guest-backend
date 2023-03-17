using BeMyGuest.Common.Utils;
using BeMyGuest.Contracts.Users;
using BeMyGuest.Domain.Users;
using BeMyGuest.Infrastructure.Persistence.Users;
using Mapster;

namespace BeMyGuest.Api.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, GetUserResponse>()
            .Map(dest => dest.UserId, src => src.Id);

        config.NewConfig<UserSnapshot, User>().ConstructUsing(src =>
            User.Create(
                src.Sk.RemoveFieldSpecifier(),
                src.FirstName,
                src.LastName,
                src.Email,
                src.Pk.RemoveFieldSpecifier(),
                src.CreatedAt,
                src.UpdatedAt));
    }
}