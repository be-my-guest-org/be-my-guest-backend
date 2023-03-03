﻿using BeMyGuest.Application.Users.Queries.GetUser;
using BeMyGuest.Contracts.Users;
using Mapster;

namespace BeMyGuest.Api.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<GetUserRequest, GetUserQuery>();
    }
}