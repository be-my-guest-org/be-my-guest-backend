using BeMyGuest.Api.Mapping;
using BeMyGuest.Common.User;

namespace BeMyGuest.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings();
        services.AddControllers();
        services.AddScoped<CurrentUserData>();

        return services;
    }
}