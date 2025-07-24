using Commerce.BuildingBlocks.Application.Interfaces.Commands;
using Commerce.BuildingBlocks.Application.Interfaces.Queries;
using Commerce.BuildingBlocks.Application.Results;

namespace Commerce.BuildingBlocks.Application.Interfaces;

public interface IDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    // Task<Result> DispatchAsync(ICommand command, CancellationToken cancellationToken = default);
    Task<Result<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}