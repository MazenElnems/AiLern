using FluentValidation;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Common.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(!_validators.Any())
            return await next(cancellationToken);

        var errors = await ValidateAsync(request, cancellationToken);

        if(errors.Any())
            return CreateValidationResult(errors);

        return await next(cancellationToken);
    }

    private async Task<List<Error>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            _validators
            .Select(async v => await v.ValidateAsync(context))
        );

        return results
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
            .ToList();
    }

    private static TResponse CreateValidationResult(List<Error> errors)
    {
        const string message = "There are one or more validation errors occurred";

        var validationErrors = errors
            .GroupBy(e => e.Title)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Message).ToArray()
            );

        TResponse response = default!;

        var resultType = typeof(TResponse);
        if (resultType == typeof(Result))   
        {
            response = (TResponse)(object)Result.ValidationFailure(
                Error.Validation("Validation Errors", message),
                validationErrors,
                message
            );
        }
        else
        {
            response = (TResponse)typeof(Result<>)
                .MakeGenericType(resultType.GenericTypeArguments[0])
                .GetMethod("ValidationFailure")!
                .Invoke(null,
                    [
                        Error.Validation("Validation Errors", message),
                        validationErrors,
                        message
                    ]
                )!;
        }

        return response;
    }
}
