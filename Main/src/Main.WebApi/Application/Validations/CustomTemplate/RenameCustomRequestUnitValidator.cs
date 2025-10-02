using Main.WebApi.Application.Commands.CustomTemplate;

namespace Main.WebApi.Application.Validations.CustomTemplate;

public class RenameCustomRequestUnitValidator : AbstractValidator<RenameCustomRequestUnitCommand>
{
    public RenameCustomRequestUnitValidator()
    {
        RuleFor(x => x.RequestUnitId)
            .NotEmpty().WithMessage("RequestUnitId is required.");

        RuleFor(x => x.RequestUnitName)
            .NotEmpty().WithMessage("RequestUnitName is required.");

        // Optionally, if versionId is existing, you need to validate version
        When(x => x.VersionId.HasValue, () =>
        {
            RuleFor(x => x.VersionId)
                .NotEmpty().WithMessage("VersionId is required when Version is provided.");

            RuleFor(x => x.Version)
                .NotEmpty().WithMessage("Version is required when VersionId is provided.");
        });
    }
}
