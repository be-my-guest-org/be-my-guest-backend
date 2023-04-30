using Amazon.DynamoDBv2;
using Amazon.Geo;
using BeMyGuest.Domain.Events;
using BeMyGuest.Domain.Users;
using BeMyGuest.Infrastructure.Configuration;
using BeMyGuest.Infrastructure.Persistence.Events;
using BeMyGuest.Infrastructure.Persistence.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeMyGuest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();

        AddDynamoDb(services, configuration);
        AddDynamoDbGeo(services);

        return services;
    }

    private static void AddDynamoDb(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<IAmazonDynamoDB, AmazonDynamoDBClient>();
        services.AddScoped<AmazonDynamoDBClient>();
        services.Configure<DynamoDbOptions>(configuration.GetSection(DynamoDbOptions.Section));
    }

    private static void AddDynamoDbGeo(IServiceCollection services)
    {
        services.AddScoped<GeoDataManager>(provider =>
        {
            var dynamoDbOptions = provider.GetRequiredService<IOptions<DynamoDbOptions>>().Value;
            var geoDataConfig = new GeoDataManagerConfiguration(provider.GetRequiredService<AmazonDynamoDBClient>(), dynamoDbOptions.TableName)
            {
                HashKeyAttributeName = "pk", RangeKeyAttributeName = "sk",
            };
            var geoDataManager = new GeoDataManager(geoDataConfig);

            return geoDataManager;
        });
    }
}