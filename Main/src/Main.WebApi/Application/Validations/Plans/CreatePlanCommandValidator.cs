using Main.WebApi.Application.Commands.Plans;

namespace Main.WebApi.Application.Validations.Plans;

public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator(ILogger<CreatePlanCommandValidator> logger)
    {
        RuleFor(x => x.PlanName)
            .NotEmpty()
            .WithMessage("計劃名稱不能為空")
            .MaximumLength(50)
            .WithMessage("計劃名稱長度不能超過 50 個字元");
        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("計劃年度不能為空")
            .InclusiveBetween(2020, 2030)
            .WithMessage("計劃年度必須在 2020 到 2030 年之間");

        // Either CustomIndicatorIdList or IndicatorIdList must be provided
        RuleFor(x => x)
            .Must(x =>
            {
                bool hasCustomIndicator = x.CustomIndicatorIdList != null && x.CustomIndicatorIdList.Length > 0;
                bool hasIndicator = x.IndicatorIdList != null && x.IndicatorIdList.Length > 0;
                return hasCustomIndicator || hasIndicator;
            })
            .WithMessage("自訂指標或指標清單必須提供其中一項");

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("{ClassName} initialized", GetType().Name);
        }
    }
}
