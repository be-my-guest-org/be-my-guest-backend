using BeMyGuest.Application.Common.PipelineBehaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BeMyGuest.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var markerType = typeof(DependencyInjection);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining(markerType);
        });

        services.AddValidatorsFromAssemblyContaining(markerType);

        return services;
    }
}