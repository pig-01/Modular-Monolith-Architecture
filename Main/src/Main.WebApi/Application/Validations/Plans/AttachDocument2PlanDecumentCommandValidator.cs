using Main.WebApi.Application.Commands.Plans;

namespace Main.WebApi.Application.Validations.Plans;

public class AttachDocument2PlanDecumentCommandValidator : AbstractValidator<AttachDocument2PlanDecumentCommand>
{
    public AttachDocument2PlanDecumentCommandValidator(ILogger<AttachDocument2PlanDecumentCommandValidator> logger)
    {

        RuleFor(x => x.DocumentId)
            .NotEmpty()
            .WithMessage("文件 ID 不能為空")
            .GreaterThan(0)
            .WithMessage("文件 ID 必須大於 0");

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("{ClassName} initialized", GetType().Name);
        }
    }
}
