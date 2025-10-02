using FluentValidation;
using FluentValidation.Results;
using Base.Domain.Exceptions;
using DataHub.Infrastructure.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataHub.Infrastructure.Application.Behaviors;

public class ValidatorBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidatorBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string typeName = request.GetGenericTypeName();
        logger.LogInformation("Validating command {CommandType}", typeName);
        List<ValidationFailure> failures = validators
                    .Select(v => v.Validate(request))
                    .SelectMany(result => result.Errors)
                    .Where(error => error != null)
                    .ToList();

        if (failures.Count != 0)
        {
            logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

            throw new DomainException(
                $"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
        }

        return await next();
    }
}
