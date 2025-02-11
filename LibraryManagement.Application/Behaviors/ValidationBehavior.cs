using System;
using FluentValidation;
using LibraryManagement.Application.Exceptions;
using MediatR;

namespace LibraryManagement.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest // Apply to requests (Commands and Queries)
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                throw new InvalidInputException("Validation failed", new ValidationException(failures)); // Or a custom exception that encapsulates validation failures
            }
        }
        return await next(); // Continue to the next behavior in the pipeline or the handler
    }
}