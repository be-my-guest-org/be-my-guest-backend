using BeMyGuest.Domain.Users;
using BeMyGuest.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BeMyGuest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}