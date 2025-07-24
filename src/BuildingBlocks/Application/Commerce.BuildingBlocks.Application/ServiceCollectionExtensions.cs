using System.Reflection;
using Commerce.BuildingBlocks.Application.Behaviors;
using Commerce.BuildingBlocks.Application.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Commerce.BuildingBlocks.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationBuildingBlocks(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddScoped<IDispatcher, Dispatcher.Dispatcher>();

        RegisterHandlers(services, assemblies);
        RegisterValidators(services, assemblies);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies)
    {
        var handlerInterfaceType = typeof(IUseCaseHandler<,>);
        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces(), (type, iface) => new { type, iface })
                .Where(x => x.iface.IsGenericType && x.iface.GetGenericTypeDefinition() == handlerInterfaceType);

            foreach (var handler in handlerTypes)
            {
                services.AddScoped(handler.iface, handler.type);
            }
        }
    }

    private static void RegisterValidators(IServiceCollection services, Assembly[] assemblies)
    {
        services.AddValidatorsFromAssemblies(assemblies);
    }
}

// public static class ServiceCollectionExtensions
// {
//     public static IServiceCollection AddApplicationBuildingBlocks(this IServiceCollection services)
//     {
//         services.AddScoped<IDispatcher, Dispatcher.Dispatcher>();

//         var assemblies = AppDomain.CurrentDomain.GetAssemblies();

//         foreach (var assembly in assemblies)
//         {
//             var handlerTypes = assembly.GetTypes()
//                 .Where(t => !t.IsAbstract && !t.IsInterface)
//                 .SelectMany(t => t.GetInterfaces()
//                     .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUseCaseHandler<,>))
//                     .Select(i => new { Interface = i, Implementation = t }));

//             foreach (var handler in handlerTypes)
//             {
//                 services.AddScoped(handler.Interface, handler.Implementation);
//             }
//         }

//         return services;
//     }
// }