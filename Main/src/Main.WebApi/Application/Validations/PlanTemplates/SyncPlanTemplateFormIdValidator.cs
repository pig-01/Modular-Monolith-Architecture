using Main.Infrastructure.Demo.Context;
using Main.WebApi.Application.Commands.PlanTemplates;

namespace Main.WebApi.Application.Validations.PlanTemplates;

public class SyncPlanTemplateFormIdValidator : AbstractValidator<SyncPlanTemplateFormIdCommand>
{
    public SyncPlanTemplateFormIdValidator(DemoContext context) => RuleFor(x => x.Version)
            .NotEmpty().WithMessage("Version is required.");
    // .MustAsync(async (version, cancellationToken) => await context.PlanTemplates.AnyAsync(pt => pt.Version == version, cancellationToken))
    // .WithMessage("The specified version does not exist.");

}
