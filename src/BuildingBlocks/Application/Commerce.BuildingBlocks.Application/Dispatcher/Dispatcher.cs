using Commerce.BuildingBlocks.Application.Behaviors;
using Commerce.BuildingBlocks.Application.Interfaces;
using Commerce.BuildingBlocks.Application.Interfaces.Commands;
using Commerce.BuildingBlocks.Application.Interfaces.Queries;
using Commerce.BuildingBlocks.Application.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Commerce.BuildingBlocks.Application.Dispatcher;


public class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    // public async Task<Result> DispatchAsync(ICommand command, CancellationToken cancellationToken = default)
    // {
    //     var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
    //     dynamic handler = _serviceProvider.GetRequiredService(handlerType);
    //     return await handler.HandleAsync((dynamic)command, cancellationToken);
    // }

    public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<IQuery<TResult>, TResult>>();
        return InvokeWithPipeline<IQuery<TResult>, TResult>(query, cancellationToken, handler.HandleAsync);
    }
    public Task<Result<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return InvokeWithPipeline<ICommand<TResult>, Result<TResult>>((dynamic)command, cancellationToken, handler.HandleAsync);
    }

    private async Task<TResponse> InvokeWithPipeline<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken,
        Func<TRequest, CancellationToken, Task<TResponse>> handler)
    {
        var behaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .ToList();

        Func<Task<TResponse>> next = () => handler(request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var current = next;
            next = () => behavior.Handle(request, cancellationToken, current);
        }

        return await next();
    }
}
