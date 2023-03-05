using BeMyGuest.Domain.Users;
using Mapster;

namespace BeMyGuest.Infrastructure.Persistence.Users;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserDto, User>();
    }
}