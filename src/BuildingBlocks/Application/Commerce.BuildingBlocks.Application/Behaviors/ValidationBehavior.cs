using Commerce.BuildingBlocks.Application.Results;
using FluentValidation;

namespace Commerce.BuildingBlocks.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<Task<TResponse>> next)
    {
        var failures = _validators
            .Select(v => v.Validate(request))
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        if (failures.Count != 0)
        {
            var responseType = typeof(TResponse);
            var resultType = typeof(Result<>).MakeGenericType(responseType.GenericTypeArguments.FirstOrDefault() ?? typeof(object));
            var failMethod = resultType.GetMethod("Fail", new[] { typeof(List<string>), typeof(int) })!;

            return (TResponse)failMethod.Invoke(null, [failures, 400])!;
        }

        return await next();
    }
}
