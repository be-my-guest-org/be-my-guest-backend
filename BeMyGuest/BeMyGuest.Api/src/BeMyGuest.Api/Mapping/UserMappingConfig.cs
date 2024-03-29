﻿using BeMyGuest.Common.Utils;
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
                src.Sk.RemoveKeyIdentifiers(),
                src.FirstName,
                src.LastName,
                src.Email,
                Guid.Parse(src.Pk.RemoveKeyIdentifiers()),
                src.CreatedAt,
                src.UpdatedAt));
    }
}