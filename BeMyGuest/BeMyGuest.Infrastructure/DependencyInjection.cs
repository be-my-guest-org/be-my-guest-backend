using Amazon.DynamoDBv2;
using BeMyGuest.Domain.Events;
using BeMyGuest.Domain.Users;
using BeMyGuest.Infrastructure.Configuration;
using BeMyGuest.Infrastructure.Persistence.Events;
using BeMyGuest.Infrastructure.Persistence.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeMyGuest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();

        services.AddScoped<IAmazonDynamoDB, AmazonDynamoDBClient>();

        services.Configure<DynamoDbOptions>(configuration.GetSection(DynamoDbOptions.Section));

        return services;
    }
}