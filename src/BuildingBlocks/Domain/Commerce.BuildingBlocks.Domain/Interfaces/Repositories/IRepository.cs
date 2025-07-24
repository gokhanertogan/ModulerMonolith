namespace Commerce.BuildingBlocks.Domain.Interfaces.Repositories;

public interface IRepository<T, TId> where T : IEntity<TId>
{

}