using FluentValidation.Results;
using Base.Domain.Exceptions;
using Main.WebApi.Extensions;

namespace Main.WebApi.Application.Behaviors;

public class ValidatorBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidatorBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string typeName = request.GetGenericTypeName();
        logger.LogInformation("Validating command {CommandType}", typeName);

        // 執行所有驗證器的非同步驗證
        IEnumerable<Task<ValidationResult>> validationTasks = validators.Select(v => v.ValidateAsync(request, cancellationToken));
        ValidationResult[] validationResults = await Task.WhenAll(validationTasks);

        // 收集所有驗證失敗的錯誤
        List<ValidationFailure> failures = [.. validationResults
                    .SelectMany(result => result.Errors)
                    .Where(error => error != null)];

        if (failures.Count != 0)
        {
            logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

            throw new InvalidException(
                $"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
        }

        return await next();
    }
}
