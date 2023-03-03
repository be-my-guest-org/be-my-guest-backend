using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BeMyGuest.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var markerType = typeof(DependencyInjection);

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining(markerType);
        });

        services.AddValidatorsFromAssemblyContaining(markerType);

        return services;
    }
}