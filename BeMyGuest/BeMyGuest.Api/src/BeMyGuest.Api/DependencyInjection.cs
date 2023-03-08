﻿using BeMyGuest.Api.Mapping;

namespace BeMyGuest.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings();
        services.AddControllers();

        return services;
    }
}