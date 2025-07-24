namespace Commerce.BuildingBlocks.Application.Interfaces.Queries;

public interface IQueryHandler<in TQuery, TResult>
    : IUseCaseHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{

}