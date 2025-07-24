namespace Commerce.BuildingBlocks.Application.Interfaces;

public interface IUseCaseHandler<in TRequest, TResult>
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}