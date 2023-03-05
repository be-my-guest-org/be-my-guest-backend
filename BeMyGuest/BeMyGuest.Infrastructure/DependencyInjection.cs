﻿using Amazon.DynamoDBv2;
using BeMyGuest.Domain.Users;
using BeMyGuest.Infrastructure.Persistence.Users;
using Microsoft.Extensions.DependencyInjection;

namespace BeMyGuest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAmazonDynamoDB, AmazonDynamoDBClient>();

        return services;
    }
}