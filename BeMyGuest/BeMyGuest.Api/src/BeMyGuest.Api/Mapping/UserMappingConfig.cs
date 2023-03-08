using BeMyGuest.Application.Users.Queries.GetUser;
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
        config.NewConfig<GetUserRequest, GetUserQuery>();
        config.NewConfig<User, GetUserResponse>()
            .Map(dest => dest.UserId, src => src.Id);

        config.NewConfig<UserSnapshot, User>()
            .Map(dest => dest.Id, src => src.Pk.RemoveFieldSpecifier())
            .Map(dest => dest.Username, src => src.Sk.RemoveFieldSpecifier());
    }
}