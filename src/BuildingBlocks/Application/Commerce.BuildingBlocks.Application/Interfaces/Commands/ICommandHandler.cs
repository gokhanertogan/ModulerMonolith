namespace Commerce.BuildingBlocks.Application.Interfaces.Commands;

public interface ICommandHandler<in TCommand>
    : IUseCaseHandler<TCommand, Result>
    where TCommand : ICommand
{

}

public interface ICommandHandler<in TCommand, TResult>
    : IUseCaseHandler<TCommand, Result<TResult>>
    where TCommand : ICommand<TResult>
{ 
    
}